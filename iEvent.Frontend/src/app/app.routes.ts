import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { IndexComponent } from './pages/index/index.component';
import { EventsComponent } from './pages/events/events.component';
import { VenuesComponent } from './pages/venues/venues.component';
import { MyBookingsComponent } from './pages/my-bookings/my-bookings.component';
import { BookTicketComponent } from './pages/book-ticket/book-ticket.component';
import { EventFormComponent } from './pages/event-form/event-form.component';
import { BookingsManagerComponent } from './pages/bookings-manager/bookings-manager.component';
import { UsersComponent } from './pages/users/users.component';

export const routes: Routes = [
  { 
    path: '', 
    component: IndexComponent 
  },
  { 
    path: 'login', 
    component: LoginComponent },
  { 
    path: 'events', 
    component: EventsComponent },
  { 
    path: 'event-form', 
    component: EventFormComponent },
  { 
    path: 'venues', 
    component: VenuesComponent },
  { 
    path: 'my-bookings', 
    component: MyBookingsComponent },
  { 
    path: 'book-ticket/:id', 
    component: BookTicketComponent },
  { 
    path: 'bookings-manager', 
    component: BookingsManagerComponent },
  { 
    path: 'users', 
    component: UsersComponent },
  { 
    path: '**', 
    redirectTo: '' 
  }
];