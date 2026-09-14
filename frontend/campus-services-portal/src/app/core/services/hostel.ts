import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  CreateHostelApplication,
  Hostel,
  HostelApplication
} from '../models/hostel.model';
import { Auth } from './auth';

@Injectable({
  providedIn: 'root'
})
export class HostelService {
  private readonly http = inject(HttpClient);
  private readonly authService = inject(Auth);

  private readonly hostelsUrl =
    'http://localhost:5266/api/hostels';

  private readonly applicationsUrl =
    'http://localhost:5266/api/hostel-applications';

  getHostels(): Observable<Hostel[]> {
    return this.http.get<Hostel[]>(this.hostelsUrl);
  }
  createHostel(data: {
    name: string;
    location: string;
  }): Observable<Hostel> {
    return this.http.post<Hostel>(this.hostelsUrl, data);
  }

  updateHostel(
    id: number,
    data: {
      name: string;
      location: string;
    }
  ): Observable<Hostel> {
    return this.http.put<Hostel>(
      `${this.hostelsUrl}/${id}`,
      data
    );
  }

  deleteHostel(id: number): Observable<any> {
    return this.http.delete(
      `${this.hostelsUrl}/${id}`
    );
  }
  getRooms(hostelId: number): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.hostelsUrl}/${hostelId}/rooms`
    );
  }

  createRoom(data: {
    hostelId: number;
    roomNumber: string;
    capacity: number;
  }): Observable<any> {
    return this.http.post<any>(
      `${this.hostelsUrl}/rooms`,
      data
    );
  }

  updateRoom(
    id: number,
    data: {
      roomNumber: string;
      capacity: number;
    }
  ): Observable<any> {
    return this.http.put<any>(
      `${this.hostelsUrl}/rooms/${id}`,
      data
    );
  }
  createApplication(
    request: CreateHostelApplication
  ): Observable<HostelApplication> {
    return this.http.post<HostelApplication>(
      this.applicationsUrl,
      request
    );
  }


  getMyApplications(): Observable<HostelApplication[]> {
    const studentId = this.getCurrentStudentId();

    return this.http.get<HostelApplication[]>(
      `${this.applicationsUrl}/student/${studentId}`
    );
  }
  getAllApplications(status?: string): Observable<HostelApplication[]> {
    const url = status
      ? `${this.applicationsUrl}?status=${encodeURIComponent(status)}`
      : this.applicationsUrl;

    return this.http.get<HostelApplication[]>(url);
  }
  updateApplicationStatus(
    id: number,
    status: 'Approved' | 'Rejected',
    rejectionReason: string = ''
  ): Observable<HostelApplication> {
    return this.http.put<HostelApplication>(
      `${this.applicationsUrl}/${id}/status`,
      {
        status,
        rejectionReason
      }
    );
  }
  private getCurrentStudentId(): number {
    const currentUser = this.authService.getCurrentUser();

    if (!currentUser?.studentId) {
      throw new Error('Student information is unavailable.');
    }

    return currentUser.studentId;
  }
}