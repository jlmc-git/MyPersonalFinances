using FinanceCore.Application.Transactions.Dtos;
using FinanceCore.Domain.Transactions;
using MediatR;

namespace FinanceCore.Application.Transactions.Commands.ClassifyTransaction;

public sealed record ClassifyTransactionCommand(Guid TransactionId) : IRequest<ClassificationResultDto>;

public sealed class ClassifyTransactionCommandHandler(
    ITransactionRepository transactionRepository,
    ITransactionClassifier transactionClassifier)
    : IRequestHandler<ClassifyTransactionCommand, ClassificationResultDto>
{
    public async Task<ClassificationResultDto> Handle(
        ClassifyTransactionCommand command,
        CancellationToken cancellationToken)
    {
        Transaction transaction = await transactionRepository.GetByIdAsync(command.TransactionId, cancellationToken)
            ?? throw new InvalidOperationException($"Transaction '{command.TransactionId}' not found.");

        var request = new ClassifyTransactionRequestDto(
            TransactionId: transaction.Id,
            Amount: transaction.Amount.Amount,
            CurrencyCode: transaction.Amount.CurrencyCode.ToString(),
            OccurredAt: transaction.OccurredAt,
            Source: transaction.Source.ToString(),
            Description: transaction.Description,
            MerchantName: transaction.Merchant?.Name);

        ClassificationResultDto result = await transactionClassifier.ClassifyTransactionAsync(request, cancellationToken);

        ApplyClassificationToTransaction(transaction, result);

        return result;
    }

    private static void ApplyClassificationToTransaction(Transaction transaction, ClassificationResultDto result)
    {
        ClassificationStatusDto main = result.MainClassification;

        switch (main.Status)
        {
            case "Classified" when main.CategoryId.HasValue && main.Confidence.HasValue:
                transaction.Classify(main.CategoryId.Value, main.Confidence.Value);
                break;

            case "Ambiguous":
                transaction.RejectClassification(main.Reason ?? "Multiple categories matched.");
                break;

            case "Unclassifiable":
                transaction.RejectClassification(main.Reason ?? "No classification could be determined.");
                break;

            default:
                transaction.MarkClassificationPending();
                break;
        }
    }
}
