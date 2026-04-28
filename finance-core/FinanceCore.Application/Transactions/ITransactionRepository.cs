using FinanceCore.Domain.Transactions;

namespace FinanceCore.Application.Transactions;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default);
}
