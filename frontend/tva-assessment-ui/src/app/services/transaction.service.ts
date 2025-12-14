import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Transaction } from '../models/transaction.model';

@Injectable({ providedIn: 'root' })
export class TransactionService {
  private readonly baseUrl = '/api/transactions';

  constructor(private http: HttpClient) {}

  getOne(code: number): Observable<Transaction> {
    return this.http.get<Transaction>(`${this.baseUrl}/${code}`);
  }

  getByAccount(accountCode: number): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(
      `${this.baseUrl}/by-account/${accountCode}`
    );
  }

  create(transaction: Transaction): Observable<Transaction> {
    return this.http.post<Transaction>(this.baseUrl, transaction);
  }

  update(transaction: Transaction): Observable<Transaction> {
    return this.http.put<Transaction>(
      `${this.baseUrl}/${transaction.code}`,
      transaction
    );
  }
}
