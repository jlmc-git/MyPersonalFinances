using System.Text.RegularExpressions;
using FinanceCore.Application.Transactions;
using FinanceCore.Application.Transactions.Dtos;
using Microsoft.Extensions.Options;

namespace FinanceCore.Infrastructure.Transactions;

public sealed class RuleBasedTransactionClassifier : ITransactionClassifier
{
    private const string ClassifiedStatus = "Classified";
    private const string AmbiguousStatus = "Ambiguous";
    private const string UnclassifiableStatus = "Unclassifiable";

    private readonly IReadOnlyList<ClassificationRule> _rules;

    public RuleBasedTransactionClassifier(IOptions<ClassificationRuleOptions> options)
    {
        _rules = options.Value.Rules;
    }

    public Task<ClassificationResultDto> ClassifyTransactionAsync(
        ClassifyTransactionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var matches = FindMatchingRules(request);

        ClassificationResultDto result = matches.Count switch
        {
            0 => Unclassifiable(),
            1 => SingleMatch(matches[0]),
            _ => Ambiguous(matches)
        };

        return Task.FromResult(result);
    }

    private List<ClassificationStatusDto> FindMatchingRules(ClassifyTransactionRequestDto request)
    {
        var matches = new List<ClassificationStatusDto>();

        foreach (var rule in _rules)
        {
            MatchResult match = MatchRule(rule, request);
            if (match.IsMatch)
            {
                matches.Add(new ClassificationStatusDto(
                    Status: ClassifiedStatus,
                    CategoryId: rule.CategoryId,
                    Confidence: match.Confidence,
                    Reason: match.Reason,
                    ReasonCode: "RuleMatched",
                    RuleId: rule.RuleId,
                    RuleName: rule.RuleName,
                    CategoryName: rule.CategoryName));
            }
        }

        return matches;
    }

    private static MatchResult MatchRule(ClassificationRule rule, ClassifyTransactionRequestDto request)
    {
        if (!MatchesCurrency(rule, request.CurrencyCode) ||
            !MatchesSource(rule, request.Source) ||
            !MatchesAmount(rule, request.Amount))
        {
            return MatchResult.NoMatch;
        }

        bool merchantMatch = MatchesAnyKeyword(rule.MerchantKeywords, request.MerchantName);
        bool merchantKeywordInDescription = MatchesAnyKeyword(rule.MerchantKeywords, request.Description);
        bool descriptionPatternMatch = MatchesDescriptionPatterns(rule.DescriptionPatterns, request.Description);
        bool descriptionMatch = merchantKeywordInDescription || descriptionPatternMatch;

        if (!merchantMatch && !descriptionMatch)
        {
            return MatchResult.NoMatch;
        }

        decimal confidence = rule.BaseConfidence;
        if (merchantMatch)
        {
            confidence += rule.MerchantMatchConfidenceBoost;
        }

        if (descriptionMatch)
        {
            confidence += rule.DescriptionMatchConfidenceBoost;
        }

        if (merchantMatch && descriptionMatch)
        {
            confidence += rule.MultiSignalConfidenceBoost;
        }

        string reason = BuildMatchReason(rule, merchantMatch, merchantKeywordInDescription, descriptionPatternMatch);
        return new MatchResult(true, ClampConfidence(confidence), reason);
    }

    private static bool MatchesAnyKeyword(List<string> keywords, string? value)
    {
        if (keywords.Count == 0 || string.IsNullOrWhiteSpace(value))
            return false;

        return keywords.Any(keyword =>
            !string.IsNullOrWhiteSpace(keyword) &&
            value.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    private static bool MatchesDescriptionPatterns(List<string> patterns, string? description)
    {
        if (patterns.Count == 0 || string.IsNullOrWhiteSpace(description))
            return false;

        return patterns.Any(pattern => SafeRegexIsMatch(description, pattern));
    }

    private static bool SafeRegexIsMatch(string value, string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));
        }
        catch (ArgumentException)
        {
            return false;
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    private static bool MatchesCurrency(ClassificationRule rule, string currencyCode)
    {
        return rule.CurrencyCodes.Count == 0 ||
            rule.CurrencyCodes.Any(code => string.Equals(code, currencyCode, StringComparison.OrdinalIgnoreCase));
    }

    private static bool MatchesSource(ClassificationRule rule, string source)
    {
        return rule.SourceNames.Count == 0 ||
            rule.SourceNames.Any(name => string.Equals(name, source, StringComparison.OrdinalIgnoreCase));
    }

    private static bool MatchesAmount(ClassificationRule rule, decimal amount)
    {
        if (rule.MinimumAmount.HasValue && amount < rule.MinimumAmount.Value)
        {
            return false;
        }

        return !rule.MaximumAmount.HasValue || amount <= rule.MaximumAmount.Value;
    }

    private static string BuildMatchReason(
        ClassificationRule rule,
        bool merchantMatch,
        bool merchantKeywordInDescription,
        bool descriptionPatternMatch)
    {
        var signals = new List<string>();

        if (merchantMatch)
        {
            signals.Add("merchant keyword");
        }

        if (merchantKeywordInDescription)
        {
            signals.Add("merchant keyword in description");
        }

        if (descriptionPatternMatch)
        {
            signals.Add("description pattern");
        }

        string ruleName = string.IsNullOrWhiteSpace(rule.RuleName) ? rule.RuleId : rule.RuleName;
        return $"Matched rule '{ruleName}' using {string.Join(", ", signals)}.";
    }

    private static decimal ClampConfidence(decimal confidence)
    {
        if (confidence < 0)
        {
            return 0;
        }

        return confidence > 1 ? 1 : confidence;
    }

    private static ClassificationResultDto Unclassifiable()
    {
        var main = new ClassificationStatusDto(
            Status: UnclassifiableStatus,
            Reason: "No classification rule matched the transaction.",
            ReasonCode: "NoRuleMatched");

        return new ClassificationResultDto(main, []);
    }

    private static ClassificationResultDto SingleMatch(ClassificationStatusDto match)
    {
        return new ClassificationResultDto(match, []);
    }

    private static ClassificationResultDto Ambiguous(List<ClassificationStatusDto> matches)
    {
        var ordered = matches.OrderByDescending(m => m.Confidence).ToList();

        var main = new ClassificationStatusDto(
            Status: AmbiguousStatus,
            Reason: $"Multiple rules matched ({ordered.Count} candidates).",
            ReasonCode: "MultipleRulesMatched");

        return new ClassificationResultDto(main, ordered);
    }

    private sealed record MatchResult(bool IsMatch, decimal Confidence, string Reason)
    {
        public static MatchResult NoMatch { get; } = new(false, 0, string.Empty);
    }
}
