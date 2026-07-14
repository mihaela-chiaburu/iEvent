import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BookingService } from '../../services/booking.service';
import { EventService } from '../../services/event.service';
import { VenueService } from '../../services/venue.service';

@Component({
  selector: 'app-my-bookings',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-bookings.component.html',
  styleUrl: './my-bookings.component.css'
})
export class MyBookingsComponent implements OnInit {

  private bookingService = inject(BookingService);
  private eventService = inject(EventService);
  private venueService = inject(VenueService);

  bookings = signal<any[]>([]);

  selectedTab = signal<'pending' | 'active' | 'all'>('pending');

  ngOnInit() {
    this.loadBookings();
  }

  loadBookings() {
    this.bookingService.getMyBookings()
      .subscribe((res: any[]) => {
        this.bookings.set(res);
        res.forEach((booking) => {
          this.eventService.getEventById(booking.eventId)
            .subscribe((event: any) => {
              booking.event = {
                ...event,
                imageUrl:
                  event.images?.find((x: any) => x.isBanner)?.url
                  ?? 'assets/placeholder.jpg'
              };

              this.venueService.getVenueById(event.venueId)
                .subscribe((venue: any) => {
                  booking.venue = venue;
                  if (booking.status !== 0) {
                      this.bookingService.getQr(booking.bookingId)
                          .subscribe((qr: any) => {
                              booking.qrCode = qr.qrCodeBase64;
                              booking.bookingCode = qr.bookingCode;
                              this.bookings.set([...this.bookings()]);
                          });
                  } else {
                      this.bookings.set([...this.bookings()]);
                  }
                });
            });
        });
        if (this.pendingBookings().length === 0) {
          this.selectedTab.set('active');
        }
      });
  }

  pendingBookings() {
    return this.bookings()
      .filter(b => b.status === 0);
  }


  activeBookings() {
    return this.bookings()
      .filter(b => b.status !== 0);
  }


  displayBookings() {

    switch(this.selectedTab()) {

      case 'pending':
        return this.pendingBookings();

      case 'active':
        return this.activeBookings();

      default:
        return this.bookings();
    }

  }



  payBooking(id:string) {

    this.bookingService.simulatePayment(id)
      .subscribe(() => {

        alert('Plata a fost efectuată!');
        this.loadBookings();

      });

  }



  cancelBooking(id:string) {

    if(confirm('Sigur dorești să renunți la acest bilet?')) {

      this.bookingService.deleteBooking(id)
        .subscribe(() => {

          this.loadBookings();

        });

    }

  }



showQr(id:string) {

  this.bookingService.getQr(id)
    .subscribe((res:any)=>{

      const booking = this.bookings()
        .find(b => b.bookingId === id);

      if(booking){

        booking.qrCode = res.qrCodeBase64;

        this.bookings.set([...this.bookings()]);

      }

    });

}

  downloadPdf(id:string) {

    this.bookingService.downloadPdf(id)
      .subscribe((blob:any)=>{

        const url = window.URL.createObjectURL(blob);

        const a = document.createElement('a');
        a.href = url;
        a.download = 'booking.pdf';
        a.click();

        window.URL.revokeObjectURL(url);

      });

  }

}