import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.css'
})
export class ResetPassword {
  private readonly formBuilder = inject(FormBuilder);
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  isSubmitting = false;
  errorMessage = '';
  successMessage = '';

  readonly email =
    this.route.snapshot.queryParamMap.get('email') ?? '';

  readonly otp =
    this.route.snapshot.queryParamMap.get('otp') ?? '';

  readonly resetForm = this.formBuilder.nonNullable.group({
    newPassword: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required, Validators.minLength(6)]]
  });

  submit(): void {
    if (this.resetForm.invalid || this.isSubmitting) {
      this.resetForm.markAllAsTouched();
      return;
    }

    const { newPassword, confirmPassword } =
      this.resetForm.getRawValue();

    if (newPassword !== confirmPassword) {
      this.errorMessage = 'Passwords do not match.';
      return;
    }

    if (!this.email || !this.otp) {
      this.errorMessage =
        'Invalid password reset request. Please start again.';
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.http.post(
      `${environment.apiUrl}/auth/reset-password`,
      {
        email: this.email,
        otp: this.otp,
        newPassword,
        confirmPassword
      }
    ).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.successMessage =
          'Password reset successfully. Redirecting to login...';

        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 1500);
      },
      error: (error) => {
        this.isSubmitting = false;
        this.errorMessage =
          error.error?.message ??
          'Unable to reset password. Please try again.';
      }
    });
  }

  backToLogin(): void {
    this.router.navigate(['/login']);
  }
}