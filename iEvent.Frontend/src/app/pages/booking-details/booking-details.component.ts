import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterModule, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { forkJoin, of, Observable } from 'rxjs';
import { catchError, switchMap, map } from 'rxjs/operators';

import { BookingService } from '../../services/booking.service';
import { EventService } from '../../services/event.service';

@Component({
  selector: 'app-booking-details',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './booking-details.component.html',
  styleUrl: './booking-details.component.css'
})
export class BookingDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private http = inject(HttpClient);
  private bookingService = inject(BookingService);
  private eventService = inject(EventService);

  bookingId = '';
  booking = signal<any>(null);
  eventDetails = signal<any>(null);
  venueDetails = signal<any>(null); 
  customerDetails = signal<any>(null);
  allAvailableTicketTypes = signal<any[]>([]); 
  
  isLoading = signal<boolean>(true);
  isUpdatingTickets = signal<boolean>(false);
  isPaying = signal<boolean>(false);

  selectedNewTicketTypeId = '';
  newTicketQuantity = 1;
  qrCodeUrl = signal<string>('');

  statusLabels: { [key: number]: { text: string; class: string } } = {
    0: { text: 'Pending', class: 'bg-yellow-100 text-yellow-800 border-yellow-200' },
    1: { text: 'Paid', class: 'bg-green-100 text-green-800 border-green-200' },
    2: { text: 'Cancelled', class: 'bg-red-100 text-red-800 border-red-200' },
    3: { text: 'Refunded', class: 'bg-gray-100 text-gray-800 border-gray-200' }
  };

  ngOnInit() {
    this.bookingId = this.route.snapshot.paramMap.get('id') || '';
    if (this.bookingId) {
      this.loadBookingDetails();
    }
  }

  loadBookingDetails() {
    this.isLoading.set(true);

    this.http.get<any>(`https://localhost:44330/api/bookings/${this.bookingId}`).pipe(
      switchMap((bookingData) => {
        this.booking.set(bookingData);
        this.qrCodeUrl.set(`https://api.qrserver.com/v1/create-qr-code/?size=150x150&data=${bookingData.bookingCode}`);

        const event$ = this.eventService.getEventById(bookingData.eventId).pipe(catchError(() => of(null)));
        
        const ticketTypes$ = this.http.get<any[]>(`https://localhost:44330/api/events/${bookingData.eventId}/ticket-types`).pipe(
          catchError(() => of([]))
        );

        const user$ = this.http.get<any>(`https://localhost:44330/api/users`, {
          params: { PageSize: '1000' } 
        }).pipe(
          map(res => {
            const usersList = res.items || res;
            if (Array.isArray(usersList)) {
              return usersList.find((u: any) => u.customerId === bookingData.customerId || u.id === bookingData.customerId) || null;
            }
            return null;
          }),
          catchError(() => of(null))
        );

        return forkJoin({ event: event$, ticketTypes: ticketTypes$, user: user$ });
      }),
    switchMap(({ event, ticketTypes, user }) => {
        const venueId = event?.venueId;
        
        const venue$ = venueId 
            ? this.http.get<any>(`https://localhost:44330/api/venues/${venueId}`).pipe(catchError(() => of(null)))
            : of(null);

        return forkJoin({
            event: of(event),
            ticketTypes: of(ticketTypes),
            user: of(user),
            venue: venue$
        });
    })
    ).subscribe({
      next: ({ event, ticketTypes, user, venue }) => {
        this.eventDetails.set(event);
        this.venueDetails.set(venue); 
        if (user) {
          this.customerDetails.set(user);
        } else {
          this.customerDetails.set({ name: 'Client Necunoscut', email: 'Fără Email' });
        }

        if (Array.isArray(ticketTypes)) {
          this.allAvailableTicketTypes.set(ticketTypes);
          this.enrichBookingTickets(ticketTypes);
        }

        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Eroare la încărcarea detaliilor rezervării:', err);
        this.isLoading.set(false);
      }
    });
  }

  enrichBookingTickets(ticketTypes: any[]) {
    const currentBooking = this.booking();
    if (!currentBooking) return;

    const enrichedTickets = currentBooking.tickets.map((t: any) => {
      const match = ticketTypes.find(tt => tt.ticketTypeId === t.ticketTypeId);
      return {
        ...t,
        ticketTypeName: match ? match.name : 'Tip Bilet Șters/Inexistent'
      };
    });

    this.booking.update(b => ({ ...b, tickets: enrichedTickets }));
  }

  updateQuantity(ticket: any, increment: number) {
    const newQty = ticket.quantity + increment;
    if (newQty < 0) return;

    this.booking.update(b => {
      const updatedTickets = b.tickets.map((t: any) => {
        if (t.bookingTicketId === ticket.bookingTicketId) {
          return { ...t, quantity: newQty };
        }
        return t;
      }).filter((t: any) => t.quantity > 0);

      const newTotal = updatedTickets.reduce((acc: number, item: any) => acc + (item.quantity * item.unitPrice), 0);
      return { ...b, tickets: updatedTickets, totalPrice: newTotal };
    });
  }

  addNewTicketType() {
    if (!this.selectedNewTicketTypeId) return;

    const matchType = this.allAvailableTicketTypes().find(t => t.ticketTypeId === this.selectedNewTicketTypeId);
    if (!matchType) return;

    this.booking.update(b => {
      const existingIndex = b.tickets.findIndex((t: any) => t.ticketTypeId === this.selectedNewTicketTypeId);
      let updatedTickets = [...b.tickets];

      if (existingIndex > -1) {
        updatedTickets[existingIndex] = {
          ...updatedTickets[existingIndex],
          quantity: updatedTickets[existingIndex].quantity + this.newTicketQuantity
        };
      } else {
        updatedTickets.push({
          bookingTicketId: 'new-' + Math.random().toString(36).substr(2, 9),
          ticketTypeId: this.selectedNewTicketTypeId,
          ticketTypeName: matchType.name,
          quantity: this.newTicketQuantity,
          unitPrice: matchType.price
        });
      }

      const newTotal = updatedTickets.reduce((acc: number, item: any) => acc + (item.quantity * item.unitPrice), 0);
      return { ...b, tickets: updatedTickets, totalPrice: newTotal };
    });

    this.selectedNewTicketTypeId = '';
    this.newTicketQuantity = 1;
  }
  
  collectAtVenue() {
    this.isPaying.set(true);
    const currentBooking = this.booking();

    const payload = {
      collectedAt: new Date().toISOString(),
      amount: currentBooking.totalPrice
    };

    this.http.patch<any>(`https://localhost:44330/api/bookings/${this.bookingId}/collect-at-venue`, payload)
      .subscribe({
        next: () => {
          this.isPaying.set(false);
          alert('Plata la locație a fost înregistrată cu succes!');
          this.loadBookingDetails();
        },
        error: (err) => {
          console.error('Eroare la încasarea plății:', err);
          this.isPaying.set(false);
          alert('A apărut o eroare la înregistrarea plății.');
        }
      });
  }

  getEventDateFormatted(): string {
    const event = this.eventDetails();
    const b = this.booking();
    if (!event || !b) return 'Data nespecificată';

    if (event.eventDates) {
      for (const ed of event.eventDates) {
        if (ed.timeSlots) {
          const found = ed.timeSlots.find((s: any) => s.timeSlotId === b.bookingTimeSlotId);
          if (found) {
            return `${ed.date} (${found.startTime} - ${found.endTime})`;
          }
        }
      }
    }
    return event.allDates?.[0] || 'Dată indisponibilă';
  }
}