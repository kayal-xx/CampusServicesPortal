import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface DashboardSummary {
  totalStudents: number;
  totalHostelApplications: number;
  totalLabBookings: number;
  totalEventRegistrations: number;
  totalComplaints: number;
  pendingComplaints: number;
  totalCertificateRequests: number;
  pendingCertificateRequests: number;
  totalFeePayments: number;
  paidFeePayments: number;
  unreadNotifications: number;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly apiUrl = `${environment.apiUrl}/Dashboard/summary`;

  constructor(private http: HttpClient) {}

  getSummary(): Observable<DashboardSummary> {
    return this.http.get<DashboardSummary>(this.apiUrl);
  }
}