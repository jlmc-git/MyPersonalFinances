using FinanceCore.Domain.Transactions;

namespace FinanceCore.Application.Transactions.Dtos;

public sealed record TransactionDto(
    Guid Id,
    MoneyDto Amount,
    DateTimeOffset OccurredAt,
    string? Description,
    Guid? MerchantId,
    MerchantDto? Merchant,
    TransactionSource Source,
    ClassificationStatusDto ClassificationStatus);
