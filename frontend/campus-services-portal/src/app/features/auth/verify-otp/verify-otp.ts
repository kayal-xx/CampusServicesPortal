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
  selector: 'app-verify-otp',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './verify-otp.html',
  styleUrl: './verify-otp.css'
})
export class VerifyOtp {
  private readonly formBuilder = inject(FormBuilder);
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  isSubmitting = false;
  errorMessage = '';

  readonly verifyForm = this.formBuilder.nonNullable.group({
    otp: [
      '',
      [
        Validators.required,
        Validators.pattern(/^\d{6}$/)
      ]
    ]
  });

  readonly email =
    this.route.snapshot.queryParamMap.get('email') ?? '';

  submit(): void {
    if (this.verifyForm.invalid || this.isSubmitting) {
      this.verifyForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    const otp = this.verifyForm.controls.otp.value.trim();

    this.http.post(
      `${environment.apiUrl}/auth/verify-otp`,
      {
        email: this.email,
        otp
      }
    ).subscribe({
      next: () => {
        this.isSubmitting = false;

        this.router.navigate(
          ['/reset-password'],
          {
            queryParams: {
              email: this.email,
              otp
            }
          }
        );
      },

      error: (error) => {
        this.isSubmitting = false;

        this.errorMessage =
          error.error?.message ??
          'Invalid or expired verification code.';
      }
    });
  }

  backToLogin(): void {
    this.router.navigate(['/login']);
  }
}