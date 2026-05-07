namespace FinanceCore.Application.Transactions.Dtos;

public sealed record ClassifyTransactionRequestDto(
    Guid TransactionId,
    decimal Amount,
    string CurrencyCode,
    DateTimeOffset OccurredAt,
    string Source,
    string? Description,
    string? MerchantName);
