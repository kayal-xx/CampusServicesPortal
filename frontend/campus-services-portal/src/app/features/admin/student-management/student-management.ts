import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

import { StudentProfile } from '../../../core/models/student.model';

@Component({
  selector: 'app-student-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './student-management.html',
  styleUrl: './student-management.css'
})
export class StudentManagement implements OnInit {

  private readonly http = inject(HttpClient);

  private readonly apiUrl = 'http://localhost:5266/api/students';

  students: StudentProfile[] = [];

  search = '';
  faculty = '';

  loading = false;
  errorMessage = '';
  successMessage = '';

  showCreateForm = false;

  newStudent = {
    fullName: '',
    indexNumber: '',
    email: '',
    faculty: '',
    contactNumber: '',
    password: ''
  };

  ngOnInit(): void {
    this.loadStudents();
  }

  loadStudents(): void {
    this.loading = true;
    this.errorMessage = '';

    const params: string[] = [];

    if (this.search.trim()) {
      params.push(`search=${encodeURIComponent(this.search.trim())}`);
    }

    if (this.faculty.trim()) {
      params.push(`faculty=${encodeURIComponent(this.faculty.trim())}`);
    }

    const url = params.length > 0
      ? `${this.apiUrl}?${params.join('&')}`
      : this.apiUrl;

    this.http.get<StudentProfile[]>(url).subscribe({
      next: (students) => {
        this.students = students;
        this.loading = false;
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Unable to load students.';
        this.loading = false;
      }
    });
  }

  openCreateForm(): void {
    this.showCreateForm = true;
    this.clearMessages();
  }

  closeCreateForm(): void {
    this.showCreateForm = false;
    this.resetForm();
  }

  createStudent(): void {
    this.clearMessages();

    if (
      !this.newStudent.fullName.trim() ||
      !this.newStudent.indexNumber.trim() ||
      !this.newStudent.email.trim() ||
      !this.newStudent.faculty.trim() ||
      !this.newStudent.contactNumber.trim() ||
      !this.newStudent.password.trim()
    ) {
      this.errorMessage = 'Please fill in all student details.';
      return;
    }

    this.loading = true;

    this.http.post<StudentProfile>(
      this.apiUrl,
      this.newStudent
    ).subscribe({
      next: (student) => {
        this.students.unshift(student);

        this.successMessage =
          'Student account created successfully.';

        this.resetForm();
        this.showCreateForm = false;
        this.loading = false;
      },
      error: (error) => {
        console.error(error);

        this.errorMessage =
          error?.error?.message ||
          'Unable to create student account.';

        this.loading = false;
      }
    });
  }

  deactivateStudent(student: StudentProfile): void {
    const confirmed = window.confirm(
      `Are you sure you want to deactivate ${student.fullName}?`
    );

    if (!confirmed) {
      return;
    }

    this.clearMessages();
    this.loading = true;

    this.http.delete(
      `${this.apiUrl}/${student.id}`
    ).subscribe({
      next: () => {
        student.isActive = false;

        this.successMessage =
          'Student account deactivated successfully.';

        this.loading = false;
      },
      error: (error) => {
        console.error(error);

        this.errorMessage =
          error?.error?.message ||
          'Unable to deactivate student.';

        this.loading = false;
      }
    });
  }

  resetForm(): void {
    this.newStudent = {
      fullName: '',
      indexNumber: '',
      email: '',
      faculty: '',
      contactNumber: '',
      password: ''
    };
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }
}