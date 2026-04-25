using FinanceCore.Domain.Shared;

namespace FinanceCore.Domain.Merchants;

public sealed class Merchant
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    private Merchant()
    {
        Name = string.Empty;
    }

    private Merchant(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }

    public static Merchant Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Merchant name is required.");
        }

        return new Merchant(name.Trim());
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Merchant name is required.");
        }

        Name = name.Trim();
    }
}
