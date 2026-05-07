namespace FinanceCore.API.Transactions;

public sealed record CreateTransactionRequest(
    long AmountInMinorUnits,
    string CurrencyCode,
    DateTimeOffset OccurredAt,
    string? Description,
    string Source);
