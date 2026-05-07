using FinanceCore.Application.Transactions;

namespace FinanceCore.Infrastructure.Transactions;

public sealed class StubLlmClient : ILlmClient
{
    public Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(string.Empty);
    }
}
