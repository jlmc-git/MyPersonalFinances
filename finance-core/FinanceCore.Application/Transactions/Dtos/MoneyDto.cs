namespace FinanceCore.Application.Transactions.Dtos;

public sealed record MoneyDto(
    long AmountInMinorUnits,
    string CurrencyCode,
    int DecimalPlaces,
    decimal Amount);
