export interface AccountSummary {
  code: number;
  accountNumber: string;
  outstandingBalance: number;
  isClosed: boolean;
}

export interface Person {
  code?: number;
  idNumber: string;
  name?: string | null;
  surname?: string | null;

  accounts?: AccountSummary[];
}

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
