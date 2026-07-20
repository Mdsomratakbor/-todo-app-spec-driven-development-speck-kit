import { Component, inject, OnInit, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatCard, MatCardHeader, MatCardTitle, MatCardContent, MatCardActions } from '@angular/material/card';
import { MatButton } from '@angular/material/button';
import { AuthService } from '../../../shared/services/auth.service';
import { NotificationService } from '../../../shared/services/notification.service';
import { UserProfile } from '../../../shared/models/auth.model';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [DatePipe, MatCard, MatCardHeader, MatCardTitle, MatCardContent, MatCardActions, MatButton],
  template: `
    <div class="profile-container">
      <mat-card class="profile-card">
        <mat-card-header>
          <mat-card-title>Profile</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          @if (loading()) {
            <p>Loading profile...</p>
          } @else if (profile()) {
            <div class="profile-field">
              <strong>Email:</strong> {{ profile()!.email }}
            </div>
            <div class="profile-field">
              <strong>Role:</strong> {{ profile()!.role }}
            </div>
            <div class="profile-field">
              <strong>Member since:</strong> {{ profile()!.createdAt | date:'mediumDate' }}
            </div>
          }
        </mat-card-content>
        <mat-card-actions>
          <button mat-raised-button color="warn" (click)="onLogout()">Logout</button>
        </mat-card-actions>
      </mat-card>
    </div>
  `,
  styles: `
    .profile-container { display: flex; justify-content: center; align-items: center; min-height: 80vh; }
    .profile-card { width: 100%; max-width: 400px; }
    .profile-field { margin-bottom: 12px; }
    mat-card-actions { justify-content: center; }
  `,
})
export class ProfileComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly notification = inject(NotificationService);

  profile = signal<UserProfile | null>(null);
  loading = signal(true);

  ngOnInit(): void {
    this.authService.getProfile().subscribe({
      next: (profile) => {
        this.profile.set(profile);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.notification.error('Failed to load profile.');
      },
    });
  }

  onLogout(): void {
    this.authService.logout().subscribe({
      error: () => this.notification.error('Logout failed.'),
    });
  }
}
