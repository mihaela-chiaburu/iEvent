import { Component, OnInit, inject, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { forkJoin, of, Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { BookingService } from '../../services/booking.service';
import { EventService } from '../../services/event.service';
import { UserService } from '../../services/user.service';

export interface BookingTicket {
  bookingTicketId: string;
  ticketTypeId: string;
  quantity: number;
  unitPrice: number;
  ticketTypeName?: string; 
}

export interface Booking {
  bookingId: string;
  bookingCode: string;
  bookingDate: string; 
  eventId: string;
  bookingTimeSlotId: string;
  customerId: string;
  
  eventName?: string;
  eventDateTimeFormatted?: string; 
  customerName?: string;
  customerEmail?: string;
  tickets: BookingTicket[];
  totalPrice: number;
  status: number;
}

@Component({
  selector: 'app-booking-manager',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './bookings-manager.component.html',
  styleUrl: './bookings-manager.component.css'
})
export class BookingManagerComponent implements OnInit {
  private bookingService = inject(BookingService);
  private eventService = inject(EventService);
  private userService = inject(UserService);
  private http = inject(HttpClient);
  private router = inject(Router);

  bookings = signal<Booking[]>([]);
  isLoading = signal<boolean>(false);
  
  searchQuery = signal<string>('');
  selectedStatus = signal<number>(-1); 
  selectedDate = signal<string>('');

  statusLabels: { [key: number]: { text: string; class: string } } = {
    0: { text: 'Pending', class: 'status-pending' },
    1: { text: 'Paid', class: 'status-paid' },
    2: { text: 'Cancelled', class: 'status-cancelled' },
    3: { text: 'Refunded', class: 'status-refunded' }
  };

  private eventCache = new Map<string, any>();
  private ticketTypeCache = new Map<string, any>();
  private userCache = new Map<string, any>();

  constructor() {
    effect(() => {
      this.applyFilters();
    });
  }

  ngOnInit() {}

  applyFilters() {
    this.isLoading.set(true);

    const statusVal = this.selectedStatus();
    const parsedStatus = statusVal === -1 ? undefined : statusVal;

    const filters = {
      status: parsedStatus,
      search: this.searchQuery().trim() || undefined,
      dateFrom: this.selectedDate() || undefined,
    };

    this.bookingService.getBookings(filters).subscribe({
      next: (response: any) => {
        let rawBookings: any[] = [];
        if (response && response.items) {
          rawBookings = response.items;
        } else if (Array.isArray(response)) {
          rawBookings = response;
        }

        if (rawBookings.length === 0) {
          this.bookings.set([]);
          this.isLoading.set(false);
          return;
        }

        this.enrichBookings(rawBookings).subscribe({
          next: (enriched) => {
            this.bookings.set(enriched);
            this.isLoading.set(false);
          },
          error: (err) => {
            console.error('Eroare la îmbogățirea datelor:', err);
            this.bookings.set(rawBookings); 
            this.isLoading.set(false);
          }
        });
      },
      error: (err) => {
        console.error('Eroare la încărcarea rezervărilor:', err);
        this.isLoading.set(false);
      }
    });
  }

  private enrichBookings(rawBookings: any[]): Observable<Booking[]> {
    const tasks$ = rawBookings.map(booking => {
      const event$ = this.getEventWithCache(booking.eventId);
      
      const user$ = this.getUserWithCache(booking.customerId);

      const ticketTasks$ = (booking.tickets || []).map((t: any) => 
        this.getTicketTypeWithCache(t.ticketTypeId).pipe(
          map(ticketDetails => ({
            ...t,
            ticketTypeName: ticketDetails ? ticketDetails.name : 'Unknown Ticket'
          }))
        )
      );

      const resolvedTickets$ = ticketTasks$.length > 0 ? forkJoin(ticketTasks$) : of([]);

      return forkJoin({ event: event$, user: user$, tickets: resolvedTickets$ }).pipe(
        map(({ event, user, tickets }) => {
          let eventDateTimeFormatted = 'Data nespecificată';
          if (event && event.eventDates) {
            outerLoop: for (const ed of event.eventDates) {
              if (ed.timeSlots) {
                for (const slot of ed.timeSlots) {
                  if (slot.timeSlotId === booking.bookingTimeSlotId) {
                    eventDateTimeFormatted = `${ed.date} (${slot.startTime} - ${slot.endTime})`;
                    break outerLoop;
                  }
                }
              }
            }
            if (eventDateTimeFormatted === 'Data nespecificată' && event.allDates && event.allDates.length > 0) {
              eventDateTimeFormatted = event.allDates[0];
            }
          }

          return {
            ...booking,
            eventName: event ? event.name : 'Eveniment Șters/Indisponibil',
            eventDateTimeFormatted: eventDateTimeFormatted,
            customerName: user ? user.name : 'Client Necunoscut',
            customerEmail: user ? user.email : 'Fără Email',
            tickets: tickets as BookingTicket[]
          } as Booking;
        })
      );
    });

    return forkJoin(tasks$);
  }

  private getEventWithCache(eventId: string): Observable<any> {
    if (!eventId) return of(null);
    if (this.eventCache.has(eventId)) {
      return of(this.eventCache.get(eventId));
    }
    return this.eventService.getEventById(eventId).pipe(
      map(res => {
        this.eventCache.set(eventId, res);
        return res;
      }),
      catchError(() => of(null)) 
    );
  }

  private getTicketTypeWithCache(ticketTypeId: string): Observable<any> {
    if (!ticketTypeId) return of(null);
    if (this.ticketTypeCache.has(ticketTypeId)) {
      return of(this.ticketTypeCache.get(ticketTypeId));
    }
    return this.http.get<any>(`https://localhost:44330/api/ticket-types/${ticketTypeId}`).pipe(
      map(res => {
        this.ticketTypeCache.set(ticketTypeId, res);
        return res;
      }),
      catchError(() => of(null))
    );
  }

private isUserListLoaded = false;

private getUserWithCache(customerId: string): Observable<any> {
  if (!customerId) return of(null);
  
  if (this.userCache.has(customerId)) {
    return of(this.userCache.get(customerId));
  }

  if (!this.isUserListLoaded) {
    return this.http.get<any>('https://localhost:44330/api/users', {
      params: {
        PageSize: '1000' 
      }
    }).pipe(
      map((res: any) => {
        this.isUserListLoaded = true;
        const usersList = res.items || res;
        
        if (Array.isArray(usersList)) {
          usersList.forEach((user: any) => {
            if (user.customerId) {
              this.userCache.set(user.customerId, user);
            }
            if (user.id) {
              this.userCache.set(user.id, user);
            }
          });
        }
        
        return this.userCache.get(customerId) || null;
      }),
      catchError((err) => {
        console.warn('Nu s-a putut încărca lista mare de utilizatori:', err);
        return of(null);
      })
    );
  }

  return of(this.userCache.get(customerId) || null);
}

  resetFilters() {
    this.searchQuery.set('');
    this.selectedStatus.set(-1);
    this.selectedDate.set('');
  }

  navigateToAddBooking() {
    this.router.navigate(['/add-booking']);
  }

  togglePaidStatus(booking: Booking) {
    const action$ = booking.status === 1 
      ? this.bookingService.markUnpaid(booking.bookingId) 
      : this.bookingService.markPaid(booking.bookingId);

    action$.subscribe({
      next: () => {
        this.bookings.update(list => 
          list.map(b => b.bookingId === booking.bookingId 
            ? { ...b, status: b.status === 1 ? 0 : 1 } 
            : b
          )
        );
      },
      error: (err) => {
        console.error('Eroare la schimbarea statusului plății:', err);
        alert('Nu s-a putut schimba statusul plății.');
      }
    });
  }

  deleteBooking(id: string) {
    if (confirm('Sigur doriți să ștergeți această rezervare?')) {
      this.bookingService.deleteBooking(id).subscribe({
        next: () => {
          this.bookings.update(list => list.filter(b => b.bookingId !== id));
        },
        error: (err) => {
          console.error(err);
          alert('Eroare la ștergerea rezervării.');
        }
      });
    }
  }
}