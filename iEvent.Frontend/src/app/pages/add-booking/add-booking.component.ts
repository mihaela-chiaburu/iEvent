import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { catchError, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { BookingService } from '../../services/booking.service';
import { EventService } from '../../services/event.service';

@Component({
  selector: 'app-add-booking',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './add-booking.component.html',
  styleUrls: ['./add-booking.component.css']
})
export class AddBookingComponent {
  private bookingService = inject(BookingService);
  private eventService = inject(EventService);
  private http = inject(HttpClient);
  private router = inject(Router);

  allEvents = signal<any[]>([]);
  filteredEvents = signal<any[]>([]);
  venuesList = signal<any[]>([]);
  ticketTypes = signal<any[]>([]);
  
  selectedEvent = signal<any>(null);
  selectedSlotId = signal<string>('');
  selectedDateId = signal<string>('');
  
  quantities = signal<{ [key: string]: number }>({});

  clientMode = signal<'existing' | 'new'>('new');
  
  newCustomer = {
    name: '',
    email: '',
    phone: ''
  };

  existingCustomerId = '';
  customerSearchQuery = '';
  foundCustomers = signal<any[]>([]);

  eventSearchQuery = '';
  isSaving = signal<boolean>(false);
  errorMessage = signal<string>('');

  constructor() {
    this.loadInitialData();
  }

  loadInitialData() {
  this.eventService.getPopularVenues().subscribe({
    next: (venues: any) => {
      this.venuesList.set(venues || []);
    },
    error: () => this.errorMessage.set('Nu s-au putut încărca locațiile.')
  });

  this.eventService.getEvents().subscribe({
    next: (response: any) => {
      let eventsArray: any[] = [];
      
      if (Array.isArray(response)) {
        eventsArray = response;
      } else if (response && Array.isArray(response.items)) {
        eventsArray = response.items; 
      } else if (response && Array.isArray(response.data)) {
        eventsArray = response.data;
      } else if (response && typeof response === 'object') {
        const possibleArray = Object.values(response).find(val => Array.isArray(val));
        if (possibleArray) {
          eventsArray = possibleArray as any[];
        }
      }

      this.allEvents.set(eventsArray);
      this.filteredEvents.set(eventsArray);
    },
    error: (err) => {
      console.error('Eroare la fetch-ul evenimentelor:', err);
      this.errorMessage.set('Nu s-au putut încărca evenimentele.');
    }
  });
}

filterEvents(query: string) {
  this.eventSearchQuery = query; 
  
  if (!query) {
    this.filteredEvents.set(this.allEvents());
    return;
  }
  
  const q = query.toLowerCase();
  this.filteredEvents.set(
    this.allEvents().filter(e => e.name?.toLowerCase().includes(q))
  );
}

  onEventSelect(eventId: string) {
    if (!eventId) return;
    
    this.selectedSlotId.set('');
    this.selectedDateId.set('');
    this.ticketTypes.set([]);
    this.quantities.set({});

    const selectedEventRaw = this.allEvents().find(e => e.eventId === eventId);
    const matchingVenue = this.venuesList().find((v: any) => v.venueId === selectedEventRaw?.venueId);
    
    this.eventService.getEventById(eventId).subscribe({
      next: (ev: any) => {
        const processedDates = ev.eventDates?.map((dateGroup: any) => {
          const formattedSlots = dateGroup.timeSlots?.map((slot: any) => {
            const prices = ev.tickets?.map((t: any) => t.price) || [0];
            const min = Math.min(...prices);
            const max = Math.max(...prices);

            return {
              ...slot,
              priceRange: min === max ? `${min} Lei` : `${min} - ${max} Lei`
            };
          });

          return {
            ...dateGroup,
            timeSlots: formattedSlots
          };
        });

        this.selectedEvent.set({
          ...ev,
          processedDates: processedDates,
          venueName: matchingVenue ? matchingVenue.name : 'Locație necunoscută'
        });
      },
      error: () => this.errorMessage.set('Eroare la încărcarea detaliilor evenimentului.')
    });

    this.eventService.getEventTicketTypes(eventId).subscribe({
      next: (tickets: any) => {
        this.ticketTypes.set(tickets || []);
        const initialQty: { [key: string]: number } = {};
        tickets.forEach((t: any) => { 
          initialQty[t.ticketTypeId] = 0; 
        });
        this.quantities.set(initialQty);
      }
    });
  }

  selectSlot(slotId: string) {
    this.selectedSlotId.set(slotId);
  }

  incrementQuantity(ticketTypeId: string, maxAvailable: number) {
    const current = this.quantities()[ticketTypeId] || 0;
    if (current < maxAvailable) {
      this.quantities.update(q => ({ ...q, [ticketTypeId]: current + 1 }));
    }
  }

  decrementQuantity(ticketTypeId: string) {
    const current = this.quantities()[ticketTypeId] || 0;
    if (current > 0) {
      this.quantities.update(q => ({ ...q, [ticketTypeId]: current - 1 }));
    }
  }

searchExistingCustomer(query: string) {
  this.customerSearchQuery = query;
  if (query.length < 3) {
    this.foundCustomers.set([]);
    return;
  }

  this.bookingService.getCustomers(query)
    .pipe(
      catchError((err) => {
        console.warn('Eroare la căutarea utilizatorilor:', err);
        return of([]);
      })
    )
    .subscribe(customers => {
      this.foundCustomers.set(customers);
    });
}

selectCustomer(customer: any) {
  this.existingCustomerId = customer.customerId || customer.id; 
  
  const displayName = customer.name || customer.fullName || customer.email || 'Client fără nume';
  const displayEmail = customer.email ? ` (${customer.email})` : '';
  
  this.customerSearchQuery = `${displayName}${displayEmail}`;
  this.foundCustomers.set([]); 
}

  submitBooking() {
    this.errorMessage.set('');
    
    const eventId = this.selectedEvent()?.eventId;
    const slotId = this.selectedSlotId();
    
    if (!eventId) {
      this.errorMessage.set('Vă rugăm să selectați un eveniment.');
      return;
    }
    if (!slotId) {
      this.errorMessage.set('Vă rugăm să alegeți data și ora evenimentului.');
      return;
    }

    const selectedTicketsPayload = Object.entries(this.quantities())
      .filter(([_, qty]) => qty > 0)
      .map(([ticketTypeId, qty]) => ({
        ticketTypeId,
        quantity: qty
      }));

    if (selectedTicketsPayload.length === 0) {
      this.errorMessage.set('Trebuie să adăugați cel puțin un bilet.');
      return;
    }

    const payload: any = {
      eventId: eventId,
      bookingTimeSlotId: slotId,
      paymentMethod: 0, 
      tickets: selectedTicketsPayload
    };

    if (this.clientMode() === 'existing') {
      if (!this.existingCustomerId) {
        this.errorMessage.set('Vă rugăm să alegeți un client existent din căutare.');
        return;
      }
      payload.customerId = this.existingCustomerId;
      payload.newCustomer = null;
    } else {
      if (!this.newCustomer.name || !this.newCustomer.email) {
        this.errorMessage.set('Numele și emailul clientului nou sunt obligatorii.');
        return;
      }
      payload.customerId = null;
      payload.newCustomer = {
        name: this.newCustomer.name,
        email: this.newCustomer.email,
        phone: this.newCustomer.phone || ''
      };
    }

    this.isSaving.set(true);

    this.bookingService.createBookingByManager(payload).subscribe({
      next: (res) => {
        this.isSaving.set(false);
        this.router.navigate(['/booking-details', res.bookingId]);
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err?.error?.message || 'A apărut o eroare neașteptată la salvarea rezervării.');
      }
    });
  }
}