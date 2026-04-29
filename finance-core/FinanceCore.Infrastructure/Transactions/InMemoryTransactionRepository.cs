using System.Collections.Concurrent;
using FinanceCore.Application.Transactions;
using FinanceCore.Domain.Transactions;

namespace FinanceCore.Infrastructure.Transactions;

public sealed class InMemoryTransactionRepository : ITransactionRepository
{
    private readonly ConcurrentDictionary<Guid, Transaction> _transactions = new();

    public Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        _transactions.TryAdd(transaction.Id, transaction);
        return Task.CompletedTask;
    }
}
