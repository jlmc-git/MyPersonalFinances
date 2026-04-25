using FinanceCore.Domain.Shared;

namespace FinanceCore.Domain.Shared.ValueObjects;

public sealed record Money
{
    public long AmountInCents { get; }
    public CurrencyCode CurrencyCode { get; }

    private Money(long amountInCents, CurrencyCode currencyCode)
    {
        if (amountInCents < 0)
        {
            throw new DomainException("Money amount cannot be negative.");
        }

        if (!Enum.IsDefined(currencyCode))
        {
            throw new DomainException("Currency code is not supported.");
        }

        AmountInCents = amountInCents;
        CurrencyCode = currencyCode;
    }

    public static Money Create(long amountInCents, CurrencyCode currencyCode) =>
        new(amountInCents, currencyCode);
}
