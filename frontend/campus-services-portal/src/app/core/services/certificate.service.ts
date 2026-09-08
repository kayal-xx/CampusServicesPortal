import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import {
  CertificateRequestItem,
  CreateCertificateRequest
} from '../models/certificate.model';

@Injectable({
  providedIn: 'root'
})
export class CertificateService {
  private readonly requestsUrl = '/api/certificate-requests';

  constructor(private http: HttpClient) {}

  getRequests(status?: string): Observable<CertificateRequestItem[]> {
    let params = new HttpParams();

    if (status) {
      params = params.set('status', status);
    }

    return this.http.get<CertificateRequestItem[]>(
      this.requestsUrl,
      { params }
    );
  }

  getStudentRequests(
    studentId: number
  ): Observable<CertificateRequestItem[]> {
    return this.http.get<CertificateRequestItem[]>(
      `${this.requestsUrl}/student/${studentId}`
    );
  }

  getRequestById(
    requestId: number
  ): Observable<CertificateRequestItem> {
    return this.http.get<CertificateRequestItem>(
      `${this.requestsUrl}/${requestId}`
    );
  }

  createRequest(
    request: CreateCertificateRequest
  ): Observable<CertificateRequestItem[]> {
    return this.http.post<CertificateRequestItem[]>(
      this.requestsUrl,
      request
    );
  }
}