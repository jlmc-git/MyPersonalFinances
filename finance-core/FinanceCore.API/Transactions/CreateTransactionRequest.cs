using FinanceCore.Domain.Shared.ValueObjects;
using FinanceCore.Domain.Transactions;

namespace FinanceCore.API.Transactions;

public sealed record CreateTransactionRequest(
    long AmountInMinorUnits,
    CurrencyCode CurrencyCode,
    DateTimeOffset OccurredAt,
    string? Description,
    TransactionSource Source);
