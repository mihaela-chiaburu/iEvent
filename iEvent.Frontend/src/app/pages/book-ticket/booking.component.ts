import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EventService } from '../../services/event.service';
import { AuthService } from '../../services/auth.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './booking.component.html',
  styleUrl: './booking.component.css'
})
export class BookingComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private eventService = inject(EventService);
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  currentStep = signal<number>(1);

  eventId = signal<string>('');
  timeSlotId = signal<string>('');
  event = signal<any>(null);
  ticketTypes = signal<any[]>([]);
  quantities = signal<{ [key: string]: number }>({});

  userData = signal({
    email: '',
    phone: '',
    fullName: ''
  });

  cardData = signal({
    cardholderName: '',
    cardNumber: '',
    expiryDate: '',
    cvv: ''
  });

  paymentMethod = signal<number>(0); 
  adminFee = 10;
  bookingResponse = signal<any>(null);

  selectedTicketsSummary = computed(() => {
    return this.ticketTypes()
      .map(t => ({ ...t, qty: this.quantities()[t.ticketTypeId] || 0 }))
      .filter(t => t.qty > 0);
  });

  subtotalAmount = computed(() => {
    return this.selectedTicketsSummary().reduce((acc, t) => acc + (t.qty * t.price), 0);
  });

  totalAmount = computed(() => {
    return this.subtotalAmount() > 0 ? this.subtotalAmount() + this.adminFee : 0;
  });

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    const slotId = this.route.snapshot.queryParamMap.get('slotId'); 

    if (id) {
      this.eventId.set(id);
      if (slotId) this.timeSlotId.set(slotId);
      this.loadBookingData(id);
    }

    const loggedUser = this.authService.currentUser();
    if (loggedUser) {
      this.userData.set({
        email: loggedUser.email || '',
        phone: '',
        fullName: ''
      });
    }
  }

  loadBookingData(id: string) {
    this.eventService.getPopularVenues().subscribe((venues: any) => {
      this.eventService.getEventById(id).subscribe((ev: any) => {
        const matchingVenue = venues?.find((v: any) => v.venueId === ev.venueId);
        this.event.set({
          ...ev,
          imageUrl: ev.images?.find((img: any) => img.isBanner)?.url || 'assets/placeholder.jpg',
          venueName: matchingVenue ? matchingVenue.name : 'Locație necunoscută'
        });
      });
    });

    this.eventService.getEventTicketTypes(id).subscribe((tickets: any) => {
      this.ticketTypes.set(tickets || []);
      const initialQty: { [key: string]: number } = {};
      tickets.forEach((t: any) => { initialQty[t.ticketTypeId] = 0; });
      this.quantities.set(initialQty);
    });
  }

  incrementQuantity(ticketTypeId: string, maxAvailable: number) {
    const current = this.quantities()[ticketTypeId] || 0;
    if (current < maxAvailable) this.quantities.update(q => ({ ...q, [ticketTypeId]: current + 1 }));
  }

  decrementQuantity(ticketTypeId: string) {
    const current = this.quantities()[ticketTypeId] || 0;
    if (current > 0) this.quantities.update(q => ({ ...q, [ticketTypeId]: current - 1 }));
  }

  goToStep2() {
    if (this.subtotalAmount() === 0) {
      alert('Vă rugăm să selectați cel puțin un bilet!');
      return;
    }
    this.currentStep.set(2);
  }

  backToStep1() { this.currentStep.set(1); }
  backToStep2() { this.currentStep.set(2); }

  handleStep2Submit() {
    const ticketsPayload = this.selectedTicketsSummary().map(t => ({
      ticketTypeId: t.ticketTypeId,
      quantity: t.qty
    }));

    const payload = {
      eventId: this.eventId(),
      bookingTimeSlotId: this.timeSlotId(),
      tickets: ticketsPayload,
      paymentMethod: Number(this.paymentMethod())
    };

    this.http.post('https://localhost:44330/api/bookings', payload).subscribe({
      next: (res: any) => {
        this.bookingResponse.set(res);

        if (this.paymentMethod() === 0) {
          this.currentStep.set(25);
        } else {
          this.currentStep.set(3);
        }
      },
      error: (err) => {
        console.error(err);
        alert('Eroare la generarea rezervării.');
      }
    });
  }

  processOnlinePayment() {
    const bookingId = this.bookingResponse()?.bookingId;
    if (!bookingId) return;

    this.http.post(`https://localhost:44330/api/bookings/${bookingId}/simulate-payment`, { shouldSucceed: true }).subscribe({
      next: () => {
        this.bookingResponse.update(current => ({
          ...current,
          status: 1
        }));
        this.currentStep.set(3);
      },
      error: (err) => {
        console.error(err);
        alert('Plata a eșuat la procesare.');
      }
    });
  }
}