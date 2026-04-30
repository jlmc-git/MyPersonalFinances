import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_ENDPOINTS } from '../api/api-endpoints';
import type { TransactionDto } from '../models/transaction.dto';
import type { CreateTransactionRequest } from '../requests/create-transaction.request';

@Injectable({
  providedIn: 'root',
})
export class TransactionsService {
  private readonly http = inject(HttpClient);

  createTransaction(request: CreateTransactionRequest): Observable<TransactionDto> {
    return this.http.post<TransactionDto>(API_ENDPOINTS.transactions, request);
  }
}
