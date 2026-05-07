namespace FinanceCore.Application.Transactions.Dtos;

public sealed record ClassificationStatusDto(
    string Status,
    Guid? CategoryId = null,
    decimal? Confidence = null,
    string? Reason = null,
    string? ReasonCode = null,
    string? RuleId = null,
    string? RuleName = null,
    string? CategoryName = null);
