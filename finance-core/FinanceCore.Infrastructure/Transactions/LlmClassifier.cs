using FinanceCore.Application.Transactions;
using FinanceCore.Application.Transactions.Dtos;

namespace FinanceCore.Infrastructure.Transactions;

public sealed class LlmClassifier : ITransactionClassifier
{
    private readonly ILlmClient _llmClient;

    public LlmClassifier(ILlmClient llmClient)
    {
        _llmClient = llmClient;
    }

    public Task<ClassificationResultDto> ClassifyTransactionAsync(
        ClassifyTransactionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = new ClassificationResultDto(
            MainClassification: new ClassificationStatusDto(
                Status: "Unclassifiable",
                Reason: "LLM integration pending.",
                ReasonCode: "LlmIntegrationPending"),
            Alternatives: []);

        return Task.FromResult(result);
    }
}
