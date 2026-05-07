using FinanceCore.Domain.Shared;

namespace FinanceCore.Domain.Transactions;

public sealed class Classification
{
    public ClassificationStatus MainClassification { get; }
    public IReadOnlyList<ClassificationStatus> Alternatives { get; }

    private Classification(ClassificationStatus mainClassification, IReadOnlyList<ClassificationStatus> alternatives)
    {
        MainClassification = mainClassification;
        Alternatives = alternatives;
    }

    public static Classification Create(ClassificationStatus mainClassification, IEnumerable<ClassificationStatus>? alternatives = null)
    {
        if (mainClassification is null)
        {
            throw new DomainException("Main classification is required.");
        }

        var alternativesList = alternatives?.ToList() ?? [];

        return new Classification(mainClassification, alternativesList.AsReadOnly());
    }
}
