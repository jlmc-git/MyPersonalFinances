using FinanceCore.Domain.Merchants;
using FinanceCore.Domain.Shared;
using FinanceCore.Domain.Shared.ValueObjects;

namespace FinanceCore.Domain.Transactions;

public sealed class Transaction
{
    public Guid Id { get; private set; }
    public Money Amount { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public string? Description { get; private set; }
    public Guid? MerchantId { get; private set; }
    public Merchant? Merchant { get; private set; }
    public TransactionSource Source { get; private set; }
    public ClassificationStatus ClassificationStatus { get; private set; }

    private Transaction()
    {
        Amount = null!;
        ClassificationStatus = null!;
    }

    private Transaction(
        Money amount,
        DateTimeOffset occurredAt,
        string? description,
        Merchant? merchant,
        TransactionSource source)
    {
        Id = Guid.NewGuid();
        Amount = amount;
        OccurredAt = occurredAt;
        Description = NormalizeOptionalText(description);
        Merchant = merchant;
        MerchantId = merchant?.Id;
        Source = source;
        ClassificationStatus = ClassificationStatus.CreatePending();
    }

    public static Transaction Create(
        Money amount,
        DateTimeOffset occurredAt,
        string? description = null,
        Merchant? merchant = null,
        TransactionSource source = TransactionSource.Manual)
    {
        if (amount is null)
        {
            throw new DomainException("Transaction amount is required.");
        }

        if (occurredAt.Offset != TimeSpan.Zero)
        {
            throw new DomainException("Transaction occurrence time must be UTC.");
        }

        if (!Enum.IsDefined(source))
        {
            throw new DomainException("Transaction source is not supported.");
        }

        return new Transaction(amount, occurredAt, description, merchant, source);
    }

    public void ChangeMerchant(Merchant? merchant)
    {
        Merchant = merchant;
        MerchantId = merchant?.Id;
    }

    public void ChangeSource(TransactionSource source)
    {
        if (!Enum.IsDefined(source))
        {
            throw new DomainException("Transaction source is not supported.");
        }

        Source = source;
    }

    public void UpdateDescription(string? description)
    {
        Description = NormalizeOptionalText(description);
    }

    public void MarkClassificationPending()
    {
        ClassificationStatus = ClassificationStatus.CreatePending();
    }

    public void Classify(Guid categoryId, decimal confidence)
    {
        ClassificationStatus = ClassificationStatus.CreateClassified(categoryId, confidence);
    }

    public void RejectClassification(string reason)
    {
        ClassificationStatus = ClassificationStatus.CreateRejected(reason);
    }

    private static string? NormalizeOptionalText(string? value)
    {
        string? trimmed = value?.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }
}

public enum TransactionSource
{
    Manual = 1,
    BankStatementImport = 2,
    BankApi = 3,
    CreditCardStatementImport = 4,
    DigitalWallet = 5,
    Cash = 6,
}
