import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  ApiMessage,
  CreateLabBooking,
  Lab,
  LabBooking
} from '../models/lab.model';

@Injectable({
  providedIn: 'root'
})
export class LabService {
  private readonly apiUrl = 'http://localhost:5266/api';

  constructor(private readonly http: HttpClient) {}

  getLabs(): Observable<Lab[]> {
    return this.http.get<Lab[]>(`${this.apiUrl}/labs`);
  }

  getMyBookings(): Observable<LabBooking[]> {
    return this.http.get<LabBooking[]>(
      `${this.apiUrl}/lab-bookings/my`
    );
  }

  createBooking(
    booking: CreateLabBooking
  ): Observable<LabBooking> {
    return this.http.post<LabBooking>(
      `${this.apiUrl}/lab-bookings`,
      booking
    );
  }

  cancelBooking(bookingId: number): Observable<ApiMessage> {
    return this.http.put<ApiMessage>(
      `${this.apiUrl}/lab-bookings/${bookingId}/cancel`,
      {}
    );
  }
}