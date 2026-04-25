using FinanceCore.Domain.Shared;

namespace FinanceCore.Domain.Shared.ValueObjects;

public sealed record Money
{
    public long AmountInMinorUnits { get; }
    public CurrencyCode CurrencyCode { get; }
    public int DecimalPlaces => GetDecimalPlaces(CurrencyCode);
    public decimal Amount => AmountInMinorUnits / GetMinorUnitFactor(CurrencyCode);

    private Money(long amountInMinorUnits, CurrencyCode currencyCode)
    {
        if (amountInMinorUnits < 0)
        {
            throw new DomainException("Money amount cannot be negative.");
        }

        if (!Enum.IsDefined(currencyCode))
        {
            throw new DomainException("Currency code is not supported.");
        }

        AmountInMinorUnits = amountInMinorUnits;
        CurrencyCode = currencyCode;
    }

    public static Money Create(long amountInMinorUnits, CurrencyCode currencyCode) =>
        new(amountInMinorUnits, currencyCode);

    public static Money FromMajorUnits(decimal amount, CurrencyCode currencyCode)
    {
        if (amount < 0)
        {
            throw new DomainException("Money amount cannot be negative.");
        }

        decimal minorUnits = amount * GetMinorUnitFactor(currencyCode);

        if (minorUnits != decimal.Truncate(minorUnits))
        {
            throw new DomainException("Money amount has more decimal places than the currency supports.");
        }

        return new Money(checked((long)minorUnits), currencyCode);
    }

    private static int GetDecimalPlaces(CurrencyCode currencyCode)
    {
        if (!Enum.IsDefined(currencyCode))
        {
            throw new DomainException("Currency code is not supported.");
        }

        return currencyCode switch
        {
            CurrencyCode.CLP or CurrencyCode.PYG => 0,
            _ => 2,
        };
    }

    private static decimal GetMinorUnitFactor(CurrencyCode currencyCode) =>
        GetDecimalPlaces(currencyCode) switch
        {
            0 => 1m,
            2 => 100m,
            _ => throw new DomainException("Currency decimal places are not supported."),
        };
}
