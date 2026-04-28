using FinanceCore.Application.Transactions;
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
    TransactionSource Source) : IRequest<CreateTransactionResponse>;

public sealed record CreateTransactionResponse(
    Guid Id,
    long AmountInMinorUnits,
    CurrencyCode CurrencyCode,
    DateTimeOffset OccurredAt,
    string? Description,
    Guid? MerchantId,
    TransactionSource Source);

public sealed class CreateTransactionCommandHandler(
    ITransactionRepository transactionRepository)
    : IRequestHandler<CreateTransactionCommand, CreateTransactionResponse>
{
    public async Task<CreateTransactionResponse> Handle(
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

        return new CreateTransactionResponse(
            transaction.Id,
            transaction.Amount.AmountInMinorUnits,
            transaction.Amount.CurrencyCode,
            transaction.OccurredAt,
            transaction.Description,
            transaction.MerchantId,
            transaction.Source);
    }
}
