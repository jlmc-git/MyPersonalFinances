namespace FinanceCore.Application.Transactions;

public sealed class ClassificationRuleOptions
{
    public const string SectionName = "ClassificationRules";

    public List<ClassificationRule> Rules { get; set; } = [];
}

public sealed class ClassificationRule
{
    public string RuleId { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public List<string> MerchantKeywords { get; set; } = [];
    public List<string> DescriptionPatterns { get; set; } = [];
    public List<string> CurrencyCodes { get; set; } = [];
    public List<string> SourceNames { get; set; } = [];
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal BaseConfidence { get; set; }
    public decimal? MinimumAmount { get; set; }
    public decimal? MaximumAmount { get; set; }
    public decimal MerchantMatchConfidenceBoost { get; set; } = 0.05m;
    public decimal DescriptionMatchConfidenceBoost { get; set; } = 0.02m;
    public decimal MultiSignalConfidenceBoost { get; set; } = 0.03m;
}
