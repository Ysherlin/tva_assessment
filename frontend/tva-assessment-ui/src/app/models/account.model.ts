export interface TransactionSummary {
  code: number;
  transactionDate: string;
  amount: number;
  description: string;
  captureDate: string;
}

export interface Account {
  code?: number;
  personCode: number;
  accountNumber: string;
  outstandingBalance: number;
  isClosed: boolean;
  transactions?: TransactionSummary[];
}
