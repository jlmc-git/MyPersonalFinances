using FinanceCore.Application.Transactions.Dtos;
using FinanceCore.Domain.Merchants;
using FinanceCore.Domain.Shared.ValueObjects;
using FinanceCore.Domain.Transactions;
using MediatR;

namespace FinanceCore.Application.Transactions.Commands.CreateTransaction;

public sealed record CreateTransactionCommand(
    long AmountInMinorUnits,
    string CurrencyCode,
    DateTimeOffset OccurredAt,
    string? Description,
    Guid? MerchantId,
    string Source) : IRequest<TransactionDto>;

public sealed class CreateTransactionCommandHandler(
    ITransactionRepository transactionRepository)
    : IRequestHandler<CreateTransactionCommand, TransactionDto>
{
    public async Task<TransactionDto> Handle(
        CreateTransactionCommand request,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<CurrencyCode>(request.CurrencyCode, true, out var currencyCode))
            throw new ArgumentException($"Invalid currency code: '{request.CurrencyCode}'.");

        if (!Enum.TryParse<TransactionSource>(request.Source, true, out var source))
            throw new ArgumentException($"Invalid transaction source: '{request.Source}'.");

        Money amount = Money.Create(request.AmountInMinorUnits, currencyCode);

        Transaction transaction = Transaction.Create(
            amount,
            request.OccurredAt,
            request.Description,
            null,
            source);

        await transactionRepository.AddAsync(transaction, cancellationToken);

        return ToDto(transaction);
    }

    private static TransactionDto ToDto(Transaction transaction) =>
        new(
            transaction.Id,
            ToDto(transaction.Amount),
            transaction.OccurredAt,
            transaction.Description,
            transaction.MerchantId,
            ToDto(transaction.Merchant),
            transaction.Source.ToString(),
            ToDto(transaction.ClassificationStatus));

    private static MoneyDto ToDto(Money money) =>
        new(
            money.AmountInMinorUnits,
            money.CurrencyCode.ToString(),
            money.DecimalPlaces,
            money.Amount);

    private static MerchantDto? ToDto(Merchant? merchant) =>
        merchant is null ? null : new MerchantDto(merchant.Id, merchant.Name);

    private static ClassificationStatusDto ToDto(ClassificationStatus status) =>
        status switch
        {
            ClassificationStatus.Pending _ => new ClassificationStatusDto(
                "Pending",
                ReasonCode: "ClassificationPending"),
            ClassificationStatus.Classified classified => new ClassificationStatusDto(
                "Classified",
                classified.CategoryId,
                classified.Confidence,
                ReasonCode: "DomainClassificationApplied"),
            ClassificationStatus.Rejected rejected => new ClassificationStatusDto(
                "Rejected",
                Reason: rejected.Reason,
                ReasonCode: "DomainClassificationRejected"),
            _ => throw new InvalidOperationException("Unsupported classification status.")
        };
}
