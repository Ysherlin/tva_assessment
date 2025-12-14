import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Person, PagedResult } from '../models/person.model';

@Injectable({ providedIn: 'root' })
export class PersonService {
  private readonly baseUrl = '/api/persons';

  constructor(private http: HttpClient) {}

  search(
    idNumber: string | null,
    surname: string | null,
    accountNumber: string | null,
    pageNumber: number,
    pageSize: number
  ): Observable<PagedResult<Person>> {

    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    if (idNumber) params = params.set('idNumber', idNumber);
    if (surname) params = params.set('surname', surname);
    if (accountNumber) params = params.set('accountNumber', accountNumber);

    return this.http.get<PagedResult<Person>>(this.baseUrl, { params });
  }

  getOne(code: number): Observable<Person> {
    return this.http.get<Person>(`${this.baseUrl}/${code}`);
  }

  create(person: Person): Observable<Person> {
    return this.http.post<Person>(this.baseUrl, person);
  }

  update(person: Person): Observable<Person> {
    return this.http.put<Person>(`${this.baseUrl}/${person.code}`, person);
  }

  delete(code: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${code}`);
  }
}
