import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.css'
})
export class ForgotPassword {
  private readonly formBuilder = inject(FormBuilder);
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  isSubmitting = false;
  errorMessage = '';
  successMessage = '';

  readonly forgotForm = this.formBuilder.nonNullable.group({
    email: [
      '',
      [
        Validators.required,
        Validators.email
      ]
    ]
  });

  submit(): void {
    if (this.forgotForm.invalid || this.isSubmitting) {
      this.forgotForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    const email = this.forgotForm.controls.email.value
      .trim()
      .toLowerCase();

    this.http.post(
      `${environment.apiUrl}/auth/forgot-password`,
      { email }
    ).subscribe({
      next: () => {
        this.isSubmitting = false;

        this.router.navigate(
          ['/verify-otp'],
          {
            queryParams: { email }
          }
        );
      },

      error: (error) => {
        this.isSubmitting = false;

        if (error.status === 0) {
          this.errorMessage =
            'Cannot connect to the server. Make sure the backend is running.';
          return;
        }

        this.errorMessage =
          error.error?.message ??
          'Unable to send verification code. Please try again.';
      }
    });
  }

  backToLogin(): void {
    this.router.navigate(['/login']);
  }
}