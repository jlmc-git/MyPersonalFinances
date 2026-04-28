export type TransactionSourceDto =
  | 'Manual'
  | 'BankStatementImport'
  | 'BankApi'
  | 'CreditCardStatementImport'
  | 'DigitalWallet'
  | 'Cash';
