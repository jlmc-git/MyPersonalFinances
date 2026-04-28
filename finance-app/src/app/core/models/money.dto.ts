import type { CurrencyCodeDto } from './currency-code.dto';

export interface MoneyDto {
  amountInMinorUnits: number;
  currencyCode: CurrencyCodeDto;
  decimalPlaces: number;
  amount: number;
}
