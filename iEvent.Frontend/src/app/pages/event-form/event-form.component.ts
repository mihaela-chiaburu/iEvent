import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Venue } from '../../models/venue.model';
import { TicketType } from '../../models/ticket-type.model';
import { EventService } from '../../services/event.service';
import { TicketTypeService } from '../../services/ticket-type.service';
import { VenueService } from '../../services/venue.service';

@Component({
  standalone: true,
  imports: [FormsModule],
  templateUrl: './event-form.component.html',
  styleUrl: './event-form.component.css'
})
export class EventFormComponent implements OnInit {
  private eventService = inject(EventService);
  private ticketService = inject(TicketTypeService);
  private venueService = inject(VenueService);
  private router = inject(Router);

  venues = signal<Venue[]>([]);
  globalTicketTypes = signal<TicketType[]>([]);

  eventData = { name: '', description: '', startDate: '', endDate: '', venueId: '', imageUrl: '', category: 0 };
  
  ticketMode = 'existing';
  selectedTicketTypeId = '';
  newTicketName = '';
  ticketPrice = 0;
  ticketQty = 0;

  ngOnInit() {
    this.venueService.getVenues().subscribe(res => this.venues.set(res));
    this.ticketService.getTicketTypes().subscribe(res => this.globalTicketTypes.set(res));
  }

  saveEvent() {
    this.eventService.createEvent(this.eventData).subscribe(newEvent => {
      const ticketPayload = {
        eventId: newEvent.eventId,
        name: this.ticketMode === 'existing' 
          ? (this.globalTicketTypes().find(t => t.ticketTypeId === this.selectedTicketTypeId)?.name || 'Ticket')
          : this.newTicketName,
        price: this.ticketPrice,
        quantityAvailable: this.ticketQty
      };

      this.ticketService.createTicketType(ticketPayload).subscribe(() => {
        alert('Event and Ticket created successfully!');
        this.router.navigate(['/events']);
      });
    });
  }
}