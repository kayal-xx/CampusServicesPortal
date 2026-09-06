import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import {
  CreateEventRegistration,
  EventItem,
  EventRegistration
} from '../models/event.model';

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private readonly apiUrl = '/api';

  constructor(private http: HttpClient) {}

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
}