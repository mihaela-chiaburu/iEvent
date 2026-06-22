import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BookingService } from '../../services/booking.service';
import { TicketType } from '../../models/ticket-type.model';
import { EventService } from '../../services/event.service';

@Component({
  standalone: true,
  imports: [FormsModule],
  templateUrl: './book-ticket.component.html',
  styleUrl: './book-ticket.component.css'
})
export class BookTicketComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private bookingService = inject(BookingService);
  private eventService = inject(EventService);
  private router = inject(Router);

  eventId = '';
  ticketTypes = signal<TicketType[]>([]);
  
  selectedTicketTypeId = '';
  quantity = 1;
  paymentMethod = 0;

  ngOnInit() {
    this.eventId = this.route.snapshot.paramMap.get('id') || '';
    if (this.eventId) {
      this.eventService.getEventTicketTypes(this.eventId).subscribe(res => {
        this.ticketTypes.set(res);
        if (res.length > 0) this.selectedTicketTypeId = res[0].ticketTypeId;
      });
    }
  }

  confirmBooking() {
    const payload = {
      eventId: this.eventId,
      tickets: [{ ticketTypeId: this.selectedTicketTypeId, quantity: this.quantity }],
      paymentMethod: Number(this.paymentMethod)
    };

    this.bookingService.createBooking(payload).subscribe({
      next: () => {
        alert('Booking finalized!');
        this.router.navigate(['/my-bookings']);
      },
      error: () => alert('Error processing booking.')
    });
  }
}