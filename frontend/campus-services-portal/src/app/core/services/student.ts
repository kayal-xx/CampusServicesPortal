import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface StudentDto {
  id: number;
  fullName: string;
  indexNumber: string;
  email: string;
  faculty: string;
  contactNumber: string;
  role: string;
  isActive: boolean;
  createdAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class Student {

  private readonly apiUrl = `${environment.apiUrl}/Students`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<StudentDto[]> {
    return this.http.get<StudentDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<StudentDto> {
    return this.http.get<StudentDto>(`${this.apiUrl}/${id}`);
  }
}