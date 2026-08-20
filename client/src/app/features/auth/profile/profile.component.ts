import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { DatePipe } from '@angular/common';
import { MatCard, MatCardHeader, MatCardTitle, MatCardContent, MatCardActions } from '@angular/material/card';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { AuthService } from '../../../shared/services/auth.service';
import { NotificationService } from '../../../shared/services/notification.service';
import { UserProfile } from '../../../shared/models/auth.model';
import { SkeletonComponent } from '../../../shared/components/loading/skeleton.component';
import { ErrorStateComponent } from '../../../shared/components/loading/error-state.component';
import { withLoadingState } from '../../../shared/utils/loading.operator';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [DatePipe, MatCard, MatCardHeader, MatCardTitle, MatCardContent, MatCardActions, MatButton, MatIcon, SkeletonComponent, ErrorStateComponent],
  template: `
    <div class="profile-container">
      <mat-card class="profile-card">
        <mat-card-header>
          <mat-card-title>Profile</mat-card-title>
        </mat-card-header>
        <mat-card-content [attr.aria-busy]="loading()">
          @if (loading()) {
            <app-skeleton variant="profile" label="Loading profile..." />
          } @else if (error()) {
            <app-error-state message="Failed to load profile." (retry)="loadProfile()" />
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
          <button mat-raised-button color="warn" (click)="onLogout()" [disabled]="loggingOut()">
            @if (loggingOut()) {
              <mat-icon class="btn-spinner" fontIcon="sync" />
            }
            Logout
          </button>
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
  private readonly destroyRef = inject(DestroyRef);

  profile = signal<UserProfile | null>(null);
  loading = signal(true);
  error = signal(false);
  loggingOut = signal(false);

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.error.set(false);
    this.authService.getProfile().pipe(
      withLoadingState(this.loading),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe({
      next: (profile) => {
        this.profile.set(profile);
      },
      error: () => {
        this.error.set(true);
      },
    });
  }

  onLogout(): void {
    if (this.loggingOut()) return;
    this.loggingOut.set(true);
    this.authService.logout().subscribe({
      error: () => {
        this.loggingOut.set(false);
        this.notification.error('Logout failed.');
      },
    });
  }
}
