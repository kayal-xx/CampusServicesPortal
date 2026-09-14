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

  constructor(private readonly http: HttpClient) { }

  getLabs(): Observable<Lab[]> {
    return this.http.get<Lab[]>(`${this.apiUrl}/labs`);
  }
  createLab(data: {
    name: string;
    location: string;
    capacity: number;
    isActive: boolean;
  }): Observable<Lab> {
    return this.http.post<Lab>(
      `${this.apiUrl}/labs`,
      data
    );
  }

  updateLab(
    id: number,
    data: {
      name: string;
      location: string;
      capacity: number;
      isActive: boolean;
    }
  ): Observable<Lab> {
    return this.http.put<Lab>(
      `${this.apiUrl}/labs/${id}`,
      data
    );
  }

  deactivateLab(id: number): Observable<ApiMessage> {
    return this.http.delete<ApiMessage>(
      `${this.apiUrl}/labs/${id}`
    );
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
  getAllBookings(status?: string): Observable<LabBooking[]> {
    const url = status
      ? `${this.apiUrl}/lab-bookings?status=${encodeURIComponent(status)}`
      : `${this.apiUrl}/lab-bookings`;

    return this.http.get<LabBooking[]>(url);
  }
  updateBookingStatus(
    bookingId: number,
    status: 'Approved' | 'Rejected',
    rejectionReason: string = ''
  ): Observable<LabBooking> {
    return this.http.put<LabBooking>(
      `${this.apiUrl}/lab-bookings/${bookingId}/status`,
      {
        status,
        rejectionReason
      }
    );
  }
}