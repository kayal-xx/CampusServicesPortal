import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import {
  CreateEvent,
  CreateEventRegistration,
  EventItem,
  EventRegistration
} from '../models/event.model';
import { environment } from '../../../environments/environment';
@Injectable({
  providedIn: 'root'
})
export class EventService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getEvents(period?: 'upcoming' | 'past'): Observable<EventItem[]> {
    const url = period
      ? `${this.apiUrl}/events?period=${period}`
      : `${this.apiUrl}/events`;

    return this.http.get<EventItem[]>(url);
  }

  getEventById(id: number): Observable<EventItem> {
    return this.http.get<EventItem>(
      `${this.apiUrl}/events/${id}`
    );
  }
  createEvent(event: CreateEvent): Observable<EventItem> {
    return this.http.post<EventItem>(
      `${this.apiUrl}/events`,
      event
    );
  }
  deleteEvent(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/events/${id}`
    );
  }

  registerForEvent(
    registration: CreateEventRegistration
  ): Observable<EventRegistration> {
    return this.http.post<EventRegistration>(
      `${this.apiUrl}/event-registrations`,
      registration
    );
  }

  getStudentRegistrations(
    studentId: number
  ): Observable<EventRegistration[]> {
    return this.http.get<EventRegistration[]>(
      `${this.apiUrl}/event-registrations/student/${studentId}`
    );
  }

  cancelRegistration(
    registrationId: number,
    studentId: number
  ): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/event-registrations/${registrationId}?studentId=${studentId}`
    );
  }
  getAdminRegistrations(): Observable<EventRegistration[]> {
    return this.http.get<EventRegistration[]>(
      `${this.apiUrl}/event-registrations/admin`
    );
  }

  approveRegistration(registrationId: number): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/event-registrations/${registrationId}/approve`,
      {}
    );
  }

  rejectRegistration(
    registrationId: number,
    reason: string
  ): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/event-registrations/${registrationId}/reject`,
      reason
    );
  }

}