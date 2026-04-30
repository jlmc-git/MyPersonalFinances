using FinanceCore.Application.Transactions;
using FinanceCore.Application.Transactions.Dtos;
using FinanceCore.Domain.Merchants;
using FinanceCore.Domain.Shared.ValueObjects;
using FinanceCore.Domain.Transactions;
using MediatR;

namespace FinanceCore.Application.Transactions.Commands.CreateTransaction;

public sealed record CreateTransactionCommand(
    long AmountInMinorUnits,
    CurrencyCode CurrencyCode,
    DateTimeOffset OccurredAt,
    string? Description,
    Merchant? Merchant,
    TransactionSource Source) : IRequest<TransactionDto>;

public sealed class CreateTransactionCommandHandler(
    ITransactionRepository transactionRepository)
    : IRequestHandler<CreateTransactionCommand, TransactionDto>
{
    public async Task<TransactionDto> Handle(
        CreateTransactionCommand request,
        CancellationToken cancellationToken)
    {
        Money amount = Money.Create(request.AmountInMinorUnits, request.CurrencyCode);

        Transaction transaction = Transaction.Create(
            amount,
            request.OccurredAt,
            request.Description,
            request.Merchant,
            request.Source);

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
            transaction.Source,
            ToDto(transaction.ClassificationStatus));

    private static MoneyDto ToDto(Money money) =>
        new(
            money.AmountInMinorUnits,
            money.CurrencyCode,
            money.DecimalPlaces,
            money.Amount);

    private static MerchantDto? ToDto(Merchant? merchant) =>
        merchant is null ? null : new MerchantDto(merchant.Id, merchant.Name);

    private static ClassificationStatusDto ToDto(ClassificationStatus status) =>
        status switch
        {
            ClassificationStatus.Pending _ => new ClassificationStatusDto("Pending"),
            ClassificationStatus.Classified classified => new ClassificationStatusDto(
                "Classified",
                classified.CategoryId,
                classified.Confidence),
            ClassificationStatus.Rejected rejected => new ClassificationStatusDto(
                "Rejected",
                Reason: rejected.Reason),
            _ => throw new InvalidOperationException("Unsupported classification status.")
        };
}
