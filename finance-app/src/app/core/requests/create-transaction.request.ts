import type { CurrencyCodeDto } from '../models/currency-code.dto';
import type { TransactionSourceDto } from '../models/transaction-source.dto';

export interface CreateTransactionRequest {
  amountInMinorUnits: number;
  currencyCode: CurrencyCodeDto;
  occurredAt: Date;
  description: string | null;
  merchantId: string | null;
  source: TransactionSourceDto;
}
