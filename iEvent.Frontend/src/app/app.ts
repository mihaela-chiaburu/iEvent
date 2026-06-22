import { Component, inject } from '@angular/core';
import { RouterLink, RouterOutlet, Router } from '@angular/router';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink],
  template: `
    <nav style="padding: 15px; background: #333; color: white; display: flex; gap: 15px; align-items: center;">
      <a routerLink="/" style="color: white; font-weight: bold; text-decoration: none;">iEvent</a>
      
      @if (auth.currentUser()) {
        @if (auth.currentUser()?.role === 'Customer') {
          <a routerLink="/events" style="color: white;">Events</a>
          <a routerLink="/venues" style="color: white;">Venues</a>
          <a routerLink="/my-bookings" style="color: white;">My Bookings</a>
        }

        @if (auth.currentUser()?.role === 'EventManager') {
          <a routerLink="/events" style="color: white;">Events</a>
          <a routerLink="/venues" style="color: white;">Venues</a>
        }

        @if (auth.currentUser()?.role === 'BookingManager') {
          <a routerLink="/bookings-manager" style="color: white;">Bookings</a>
        }

        @if (auth.currentUser()?.role === 'SuperAdmin') {
          <a routerLink="/events" style="color: white;">Events</a>
          <a routerLink="/venues" style="color: white;">Venues</a>
          <a routerLink="/bookings-manager" style="color: white;">Bookings</a>
          <a routerLink="/users" style="color: white;">Users</a>
        }

        <span style="margin-left: auto;">{{ auth.currentUser()?.email }} ({{ auth.currentUser()?.role }})</span>
        <button (click)="logout()" style="background: red; color: white; border: none; padding: 5px 10px; cursor: pointer;">Logout</button>
      } @else {
        <a routerLink="/login" style="color: white; margin-left: auto;">Login</a>
      }
    </nav>

    <main style="padding: 20px;">
      <router-outlet></router-outlet>
    </main>
  `
})
export class App {
  auth = inject(AuthService);
  private router = inject(Router);

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}