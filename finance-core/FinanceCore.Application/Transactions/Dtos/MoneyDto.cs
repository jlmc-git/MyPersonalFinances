using FinanceCore.Domain.Shared.ValueObjects;

namespace FinanceCore.Application.Transactions.Dtos;

public sealed record MoneyDto(
    long AmountInMinorUnits,
    CurrencyCode CurrencyCode,
    int DecimalPlaces,
    decimal Amount);
