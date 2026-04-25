using FinanceCore.Domain.Shared;

namespace FinanceCore.Domain.Transactions;

public abstract class ClassificationStatus
{
    private ClassificationStatus()
    {
    }

    public sealed class Pending : ClassificationStatus
    {
        internal Pending()
        {
        }
    }

    public sealed class Classified : ClassificationStatus
    {
        public Guid CategoryId { get; }
        public decimal Confidence { get; }

        internal Classified(Guid categoryId, decimal confidence)
        {
            if (categoryId == Guid.Empty)
            {
                throw new DomainException("Category id is required.");
            }

            if (confidence is < 0 or > 1)
            {
                throw new DomainException("Classification confidence must be between 0 and 1.");
            }

            CategoryId = categoryId;
            Confidence = confidence;
        }
    }

    public sealed class Rejected : ClassificationStatus
    {
        public string Reason { get; }

        internal Rejected(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                throw new DomainException("Rejection reason is required.");
            }

            Reason = reason.Trim();
        }
    }

    public static ClassificationStatus CreatePending() => new Pending();

    public static ClassificationStatus CreateClassified(Guid categoryId, decimal confidence) =>
        new Classified(categoryId, confidence);

    public static ClassificationStatus CreateRejected(string reason) =>
        new Rejected(reason);
}
