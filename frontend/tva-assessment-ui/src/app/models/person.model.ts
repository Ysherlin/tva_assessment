export interface Person {
  code?: number;
  idNumber: string;
  name?: string | null;
  surname?: string | null;
}

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
