using FinanceCore.Domain.Shared;

namespace FinanceCore.Domain.Categories;

public sealed class Category
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    private Category()
    {
        Name = string.Empty;
    }

    private Category(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }

    public static Category Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Category name is required.");
        }

        return new Category(name.Trim());
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Category name is required.");
        }

        Name = name.Trim();
    }
}
