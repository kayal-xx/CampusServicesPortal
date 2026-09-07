import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  OnInit,
  inject
} from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { finalize } from 'rxjs';
import { StudentProfile } from '../../../core/models/student.model';
import { StudentService } from '../../../core/services/student';
import { Navbar } from '../../../shared/navbar/navbar';
@Component({
  selector: 'app-student-list',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    Navbar
  ],
  templateUrl: './student-list.html',
  styleUrl: './student-list.css'
})
export class StudentList implements OnInit {
  private readonly studentService = inject(StudentService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly changeDetector = inject(ChangeDetectorRef);
  student: StudentProfile | null = null;

  isLoading = true;
  isEditing = false;
  isSaving = false;

  errorMessage = '';
  successMessage = '';

  readonly profileForm = this.formBuilder.nonNullable.group({
    fullName: [
      '',
      [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(100)
      ]
    ],
    email: [
      '',
      [
        Validators.required,
        Validators.email
      ]
    ],
    faculty: [
      '',
      [
        Validators.required,
        Validators.maxLength(100)
      ]
    ],
    contactNumber: [
      '',
      [
        Validators.required,
        Validators.pattern(/^[0-9+\-\s]{9,15}$/)
      ]
    ]
  });

  ngOnInit(): void {
    this.loadProfile();
  }

  get initials(): string {
    if (!this.student?.fullName) {
      return 'ST';
    }

    return this.student.fullName
      .split(' ')
      .filter(part => part.length > 0)
      .slice(0, 2)
      .map(part => part.charAt(0).toUpperCase())
      .join('');
  }

  loadProfile(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.studentService.getMyProfile()
      .pipe(
        finalize(() => {
          this.isLoading = false;
          this.changeDetector.markForCheck();
        })
      )
      .subscribe({
        next: student => {
          this.student = student;
          this.fillForm(student);
        },
        error: error => {
          if (error.status === 401) {
            this.errorMessage =
              'Your session has expired. Please sign in again.';
            return;
          }

          if (error.status === 403) {
            this.errorMessage =
              'You do not have permission to view this profile.';
            return;
          }

          this.errorMessage =
            error.error?.message ??
            'Unable to load your profile.';
        }
      });
  }

  startEditing(): void {
    if (!this.student) {
      return;
    }

    this.fillForm(this.student);
    this.isEditing = true;
    this.errorMessage = '';
    this.successMessage = '';
  }

  cancelEditing(): void {
    if (this.student) {
      this.fillForm(this.student);
    }

    this.isEditing = false;
    this.errorMessage = '';
  }

  saveProfile(): void {
    if (this.profileForm.invalid || this.isSaving) {
      this.profileForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';
    this.successMessage = '';

    const formValue = this.profileForm.getRawValue();

    this.studentService.updateMyProfile({
      fullName: formValue.fullName.trim(),
      email: formValue.email.trim(),
      faculty: formValue.faculty.trim(),
      contactNumber: formValue.contactNumber.trim()
    })
      .pipe(
        finalize(() => {
          this.isSaving = false;
          this.changeDetector.markForCheck();
        })
      )
      .subscribe({
        next: student => {
          this.student = student;
          this.fillForm(student);
          this.isEditing = false;
          this.successMessage =
            'Profile updated successfully.';
        },
        error: error => {
          if (error.status === 409) {
            this.errorMessage =
              error.error?.message ??
              'This email address is already in use.';
            return;
          }

          this.errorMessage =
            error.error?.message ??
            'Unable to update your profile.';
        }
      });
  }

  private fillForm(student: StudentProfile): void {
    this.profileForm.setValue({
      fullName: student.fullName,
      email: student.email,
      faculty: student.faculty,
      contactNumber: student.contactNumber
    });
  }
}