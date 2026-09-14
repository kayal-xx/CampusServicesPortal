import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  StudentProfile,
  UpdateStudentProfile
} from '../models/student.model';
import { Auth } from './auth';

@Injectable({
  providedIn: 'root'
})
export class StudentService {
  private readonly http = inject(HttpClient);
  private readonly authService = inject(Auth);
  private readonly apiUrl = 'http://localhost:5266/api/students';

  getMyProfile(): Observable<StudentProfile> {
    const studentId = this.getCurrentStudentId();

    return this.http.get<StudentProfile>(
      `${this.apiUrl}/${studentId}`
    );
  }

  updateMyProfile(
    profile: UpdateStudentProfile
  ): Observable<StudentProfile> {
    const studentId = this.getCurrentStudentId();

    return this.http.put<StudentProfile>(
      `${this.apiUrl}/${studentId}`,
      profile
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