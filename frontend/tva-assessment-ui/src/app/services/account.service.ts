import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Account } from '../models/account.model';

@Injectable({ providedIn: 'root' })
export class AccountService {
  private readonly baseUrl = '/api/accounts';

  constructor(private http: HttpClient) {}

  getOne(code: number): Observable<Account> {
    return this.http.get<Account>(`${this.baseUrl}/${code}`);
  }

  getByPerson(personCode: number): Observable<Account[]> {
    return this.http.get<Account[]>(`${this.baseUrl}/by-person/${personCode}`);
  }

  create(account: Account): Observable<Account> {
    return this.http.post<Account>(this.baseUrl, account);
  }

  update(account: Account): Observable<Account> {
    return this.http.put<Account>(`${this.baseUrl}/${account.code}`, account);
  }

  close(code: number): Observable<Account> {
    return this.http.post<Account>(`${this.baseUrl}/${code}/close`, {});
  }

  reopen(code: number): Observable<Account> {
    return this.http.post<Account>(`${this.baseUrl}/${code}/reopen`, {});
  }
}
