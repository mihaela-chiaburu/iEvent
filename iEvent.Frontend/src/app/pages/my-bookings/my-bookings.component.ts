import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Booking } from '../../models/booking.model';
import { SlicePipe } from '@angular/common';
import { BookingService } from '../../services/booking.service';

@Component({
  standalone: true,
  imports: [FormsModule, SlicePipe],
  templateUrl: './my-bookings.component.html',
  styleUrl: './my-bookings.component.css'
})
export class MyBookingsComponent implements OnInit {
  private bookingService = inject(BookingService);
  bookings = signal<Booking[]>([]);
  editingBookingId = signal<string | null>(null);

  ngOnInit() { this.loadBookings(); }

  loadBookings() {
    this.bookingService.getMyBookings().subscribe(res => this.bookings.set(res));
  }

  saveEdit(booking: Booking) {
    this.bookingService.updateBooking(booking.bookingId, { status: Number(booking.status) }).subscribe(() => {
      alert('Booking updated successfully!');
      this.editingBookingId.set(null);
      this.loadBookings();
    });
  }

  cancelBooking(id: string) {
    if (confirm('Are you sure you want to delete this booking?')) {
      this.bookingService.deleteBooking(id).subscribe(() => {
        alert('Booking deleted!');
        this.loadBookings();
      });
    }
  }
}