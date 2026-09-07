import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface FeePayment {
  id: number;
  studentId: number;
  feeType: string;
  amount: number;
  isPaid: boolean;
  paidAt: string | null;
}

export interface CreateFeePayment {
  studentId: number;
  feeType: string;
  amount: number;
}

@Injectable({
  providedIn: 'root'
})
export class Fee {

  private readonly apiUrl = `${environment.apiUrl}/FeePayments`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<FeePayment[]> {
    return this.http.get<FeePayment[]>(this.apiUrl);
  }

  getById(id: number): Observable<FeePayment> {
    return this.http.get<FeePayment>(`${this.apiUrl}/${id}`);
  }

  getByStudentId(studentId: number): Observable<FeePayment[]> {
    return this.http.get<FeePayment[]>(
      `${this.apiUrl}/student/${studentId}`
    );
  }

  create(data: CreateFeePayment): Observable<FeePayment> {
    return this.http.post<FeePayment>(this.apiUrl, data);
  }

  updateStatus(id: number, isPaid: boolean): Observable<FeePayment> {
    return this.http.put<FeePayment>(
      `${this.apiUrl}/${id}/status`,
      { isPaid }
    );
  }
}