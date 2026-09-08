import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Router } from '@angular/router';
import { Auth } from '../../../core/services/auth';

@Component({
  selector: 'app-login',
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(Auth);
  private readonly router = inject(Router);

  isSubmitting = false;
  showPassword = false;
  errorMessage = '';

  readonly loginForm = this.formBuilder.nonNullable.group({
    email: [
      '',
      [
        Validators.required,
        Validators.email
      ]
    ],
    password: [
      '',
      [
        Validators.required,
        Validators.minLength(6)
      ]
    ],
    rememberMe: [false]
  });

  get emailControl() {
    return this.loginForm.controls.email;
  }

  get passwordControl() {
    return this.loginForm.controls.password;
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  submit(): void {
    if (this.loginForm.invalid || this.isSubmitting) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    const formValue = this.loginForm.getRawValue();

    this.authService.login({
      email: formValue.email.trim(),
      password: formValue.password.trim()
    }).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.router.navigate(['/dashboard']);
      },
      error: error => {
        this.isSubmitting = false;

        if (error.status === 401) {
          this.errorMessage = 'Invalid email or password.';
          return;
        }

        if (error.status === 0) {
          this.errorMessage =
            'Cannot connect to the server. Make sure the backend is running.';
          return;
        }

        this.errorMessage =
          error.error?.message ??
          'Login failed. Please try again.';
      }
    });
  }
}