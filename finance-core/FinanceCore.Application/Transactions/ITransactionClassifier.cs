using FinanceCore.Application.Transactions.Dtos;

namespace FinanceCore.Application.Transactions;

public interface ITransactionClassifier
{
    Task<ClassificationResultDto> ClassifyTransactionAsync(
        ClassifyTransactionRequestDto request,
        CancellationToken cancellationToken = default);
}
