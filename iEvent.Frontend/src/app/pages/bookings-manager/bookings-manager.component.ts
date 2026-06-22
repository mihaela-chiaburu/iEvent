import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BookingService } from '../../services/booking.service';
import { Booking } from '../../models/booking.model';
import { TicketType } from '../../models/ticket-type.model';
import { TicketTypeService } from '../../services/ticket-type.service';

@Component({
  standalone: true,
  imports: [FormsModule],
  templateUrl: './bookings-manager.component.html',
  styleUrl: './bookings-manager.component.css'
})
export class BookingsManagerComponent implements OnInit {
  private bookingService = inject(BookingService);
  private ticketService = inject(TicketTypeService);
  allBookings = signal<Booking[]>([]);
  ticketTypes = signal<TicketType[]>([]);

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.bookingService.getAllBookings().subscribe(res => this.allBookings.set(res));
    this.ticketService.getTicketTypes().subscribe(res => this.ticketTypes.set(res));
  }

  markAsPaid(id: string) {
    this.bookingService.markPaid(id).subscribe(() => { alert('Marked as paid'); this.loadData(); });
  }

  markAsUnpaid(id: string) {
    this.bookingService.markUnpaid(id).subscribe(() => { alert('Marked as unpaid'); this.loadData(); });
  }

  viewDetails(id: string) {
    this.bookingService.getBookingById(id).subscribe(b => {
      alert(`Booking Details:\nCode: ${b.bookingCode}\nTotal Tickets: ${b.tickets.length}\nPrice: ${b.totalPrice}`);
    });
  }

  updateStock(ticket: TicketType) {
    this.ticketService.updateTicketType(ticket.ticketTypeId, {
      eventId: ticket.eventId,
      name: ticket.name,
      price: ticket.price,
      quantityAvailable: Number(ticket.quantityAvailable)
    }).subscribe(() => alert('Stock updated successfully!'));
  }
}