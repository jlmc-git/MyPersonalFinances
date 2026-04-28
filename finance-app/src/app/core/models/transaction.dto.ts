import type { ClassificationStatusDto } from './classification-status.dto';
import type { MerchantDto } from './merchant.dto';
import type { MoneyDto } from './money.dto';
import type { TransactionSourceDto } from './transaction-source.dto';

export interface TransactionDto {
  id: string;
  amount: MoneyDto;
  occurredAt: Date;
  description: string | null;
  merchantId: string | null;
  merchant: MerchantDto | null;
  source: TransactionSourceDto;
  classificationStatus: ClassificationStatusDto;
}
