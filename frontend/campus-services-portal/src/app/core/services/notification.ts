import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface NotificationItem {
  id: number;
  studentId: number;
  message: string;
  isRead: boolean;
  createdAt: string;
}

export interface CreateNotification {
  studentId: number;
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class Notification {

  private readonly apiUrl = `${environment.apiUrl}/Notifications`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<NotificationItem[]> {
    return this.http.get<NotificationItem[]>(this.apiUrl);
  }

  getById(id: number): Observable<NotificationItem> {
    return this.http.get<NotificationItem>(`${this.apiUrl}/${id}`);
  }

  getByStudentId(studentId: number): Observable<NotificationItem[]> {
    return this.http.get<NotificationItem[]>(
      `${this.apiUrl}/student/${studentId}`
    );
  }

  create(data: CreateNotification): Observable<NotificationItem> {
    return this.http.post<NotificationItem>(this.apiUrl, data);
  }

  updateReadStatus(
    id: number,
    isRead: boolean
  ): Observable<NotificationItem> {
    return this.http.put<NotificationItem>(
      `${this.apiUrl}/${id}/read-status`,
      { isRead }
    );
  }
}