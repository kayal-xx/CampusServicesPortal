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

  private getCurrentStudentId(): number {
    const currentUser = this.authService.getCurrentUser();

    if (!currentUser?.studentId) {
      throw new Error('Student information is unavailable.');
    }

    return currentUser.studentId;
  }
}