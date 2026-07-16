import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { IndexComponent } from './pages/index/index.component';
import { EventsComponent } from './pages/events/events.component';
import { VenuesComponent } from './pages/venues/venues.component';
import { MyBookingsComponent } from './pages/my-bookings/my-bookings.component';
import { BookingComponent } from './pages/book-ticket/booking.component';
import { EventFormComponent } from './pages/event-form/event-form.component';
import { BookingManagerComponent } from './pages/bookings-manager/bookings-manager.component';
import { UsersComponent } from './pages/users/users.component';
import { EventDetailsComponent } from './pages/event-details/event-details.component';
import { VenueDetailsComponent } from './pages/venue-details/venue-details.component';
import { CreateEventComponent } from './pages/create-event/create-event.component';
import { CreateVenueComponent } from './pages/create-venue/create-venue.component';
import { BookingDetailsComponent } from './pages/booking-details/booking-details.component';
import { AddBookingComponent } from './pages/add-booking/add-booking.component';

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
    path: 'booking/:id', 
    component: BookingComponent },
  { 
    path: 'bookings-manager', 
    component: BookingManagerComponent },
  { 
    path: 'users', 
    component: UsersComponent },
  { 
    path: 'event-details/:id', 
    component: EventDetailsComponent },
  { 
    path: 'venue-details/:id', 
    component: VenueDetailsComponent },
  { 
    path: 'create-event', 
    component: CreateEventComponent },
  { 
    path: 'create-venue/:id', 
    component: CreateVenueComponent },
  { 
    path: 'bookings/:id', 
    component: BookingDetailsComponent },
  { 
    path: 'add-booking', 
    component: AddBookingComponent },
  { 
    path: '**', 
    redirectTo: '' 
  }
];