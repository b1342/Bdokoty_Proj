import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthApiService } from '../../api/auth-api.service';
import { AuthService } from '../../../../core/auth/auth.service';
import { ApiError } from '../../../../core/models/api-error.model';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
})
export class LoginPage {
  private readonly fb = inject(FormBuilder);
  private readonly authApiService = inject(AuthApiService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    rememberMe: [false],
  });

  readonly isLoading = signal(false);
  readonly showPassword = signal(false);
  readonly serverError = signal<string | null>(null);

  togglePassword(): void {
    this.showPassword.update((v) => !v);
  }

  isFieldInvalid(name: string): boolean {
    const control = this.form.get(name);
    return !!(control?.invalid && (control.dirty || control.touched));
  }

  fieldError(name: string): string | null {
    const control = this.form.get(name);
    if (!control?.errors || !(control.dirty || control.touched)) return null;
    if (control.errors['required']) return 'שדה חובה';
    if (control.errors['email']) return 'כתובת אימייל לא תקינה';
    if (control.errors['minlength']) return 'הסיסמה חייבת להכיל לפחות 6 תווים';
    return null;
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.serverError.set(null);

    const { email, password } = this.form.value;

    this.authApiService.login({ email: email!, password: password! }).subscribe({
      next: (response) => {
        this.authService.setSessionFromAuthResponse(response);
        this.router.navigate(['/works']);
      },
      error: (err: ApiError) => {
        this.serverError.set(err.message);
        this.isLoading.set(false);
      },
    });
  }
}
