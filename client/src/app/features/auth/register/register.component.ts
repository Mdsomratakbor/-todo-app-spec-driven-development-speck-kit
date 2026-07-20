import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatCard, MatCardHeader, MatCardTitle, MatCardSubtitle, MatCardContent, MatCardActions } from '@angular/material/card';
import { MatInput } from '@angular/material/input';
import { MatButton } from '@angular/material/button';
import { MatError, MatFormField, MatLabel } from '@angular/material/form-field';
import { AuthService } from '../../../shared/services/auth.service';
import { NotificationService } from '../../../shared/services/notification.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink, MatCard, MatCardHeader, MatCardTitle, MatCardSubtitle, MatCardContent, MatCardActions, MatInput, MatButton, MatFormField, MatLabel, MatError],
  template: `
    <div class="auth-container">
      <mat-card class="auth-card">
        <mat-card-header>
          <mat-card-title>Register</mat-card-title>
          <mat-card-subtitle>Create a new account</mat-card-subtitle>
        </mat-card-header>
        <mat-card-content>
          <form (ngSubmit)="onSubmit()" #registerForm="ngForm">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Email</mat-label>
              <input matInput [(ngModel)]="email" name="email" type="email" required email maxlength="254" #emailField="ngModel" />
              @if (emailField.invalid && emailField.touched) {
                <mat-error>Please enter a valid email address.</mat-error>
              }
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Password</mat-label>
              <input matInput [(ngModel)]="password" name="password" type="password" required minlength="8" #passwordField="ngModel" />
              @if (passwordField.invalid && passwordField.touched) {
                <mat-error>Password must be at least 8 characters with uppercase, lowercase, and a digit.</mat-error>
              }
            </mat-form-field>

            <button mat-raised-button color="primary" type="submit" class="full-width" [disabled]="loading() || registerForm.invalid">
              {{ loading() ? 'Registering...' : 'Register' }}
            </button>
          </form>
        </mat-card-content>
        <mat-card-actions>
          <a mat-button routerLink="/login">Already have an account? Login</a>
        </mat-card-actions>
      </mat-card>
    </div>
  `,
  styles: `
    .auth-container { display: flex; justify-content: center; align-items: center; min-height: 80vh; }
    .auth-card { width: 100%; max-width: 400px; }
    .full-width { width: 100%; }
    mat-card-content { display: flex; flex-direction: column; gap: 8px; }
    mat-card-actions { justify-content: center; }
  `,
})
export class RegisterComponent {
  private readonly authService = inject(AuthService);
  private readonly notification = inject(NotificationService);
  private readonly router = inject(Router);

  email = '';
  password = '';
  loading = signal(false);

  onSubmit(): void {
    if (this.loading()) return;
    this.loading.set(true);

    this.authService.register({ email: this.email, password: this.password }).subscribe({
      next: () => {
        this.notification.success('Registration successful!');
        this.router.navigate(['/todos']);
      },
      error: (err) => {
        this.loading.set(false);
        const message = err.error?.detail || 'Registration failed. Please try again.';
        this.notification.error(message);
      },
    });
  }
}
