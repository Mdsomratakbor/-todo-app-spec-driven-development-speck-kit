import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { MatToolbar } from '@angular/material/toolbar';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { AuthService } from './shared/services/auth.service';
import { routeAnimation } from './shared/animations/route.animations';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, MatToolbar, MatButton, MatIcon],
  templateUrl: './app.html',
  styleUrl: './app.scss',
  animations: [routeAnimation],
})
export class App {
  protected readonly title = signal('TodoApp');
  protected readonly authService = inject(AuthService);
  protected readonly loggingOut = signal(false);

  protected onLogout(): void {
    if (this.loggingOut()) return;
    this.loggingOut.set(true);
    this.authService.signOut();
  }

  protected getRouteAnimation(): number {
    return 1;
  }
}
