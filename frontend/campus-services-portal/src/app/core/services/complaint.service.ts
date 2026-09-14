import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import {
  ComplaintCategory,
  ComplaintItem,
  CreateComplaint
} from '../models/complaint.model';

import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ComplaintService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getStudentComplaints(studentId: number): Observable<ComplaintItem[]> {
    return this.http.get<ComplaintItem[]>(
      `${this.apiUrl}/complaints/student/${studentId}`
    );
  }

  getComplaintById(id: number): Observable<ComplaintItem> {
    return this.http.get<ComplaintItem>(
      `${this.apiUrl}/complaints/${id}`
    );
  }

  getComplaintsByStatus(status?: string): Observable<ComplaintItem[]> {
    const params = status
      ? new HttpParams().set('status', status)
      : undefined;

    return this.http.get<ComplaintItem[]>(
      `${this.apiUrl}/complaints`,
      { params }
    );
  }

  getActiveCategories(): Observable<ComplaintCategory[]> {
    return this.http.get<ComplaintCategory[]>(
      `${this.apiUrl}/complaint-categories`
    );
  }

  createComplaint(
    complaint: CreateComplaint
  ): Observable<ComplaintItem> {
    return this.http.post<ComplaintItem>(
      `${this.apiUrl}/complaints`,
      complaint
    );
  }

  updateComplaintStatus(
    id: number,
    status: 'Pending' | 'In Progress' | 'Resolved' | 'Rejected',
    resolutionNote: string = ''
  ): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/complaints/${id}/status`,
      {
        status,
        resolutionNote
      }
    );
  }
}