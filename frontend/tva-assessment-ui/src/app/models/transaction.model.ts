export interface Transaction {
  code?: number;
  accountCode: number;
  transactionDate: Date;
  amount: number;
  description: string;
  captureDate?: Date;
}
