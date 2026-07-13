import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { EventService } from '../../services/event.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-event-details',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './event-details.component.html',
  styleUrl: './event-details.component.css'
})
export class EventDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private eventService = inject(EventService);
  private http = inject(HttpClient); 

  eventId = signal<string>('');
  event = signal<any>(null);
  similarEvents = signal<any[]>([]);
  venuesList = signal<any[]>([]);

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.eventId.set(id);
        this.loadData(id);
      }
    });
  }

  loadData(id: string) {
    this.eventService.getPopularVenues().subscribe((venues: any) => {
      this.venuesList.set(venues || []);

      this.eventService.getEventById(id).subscribe((ev: any) => {
        const matchingVenue = this.venuesList().find((v: any) => v.venueId === ev.venueId);
        
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

        this.event.set({
          ...ev,
          imageUrl: ev.images?.find((img: any) => img.isBanner)?.url || 'assets/placeholder.jpg',
          venueName: matchingVenue ? matchingVenue.name : 'Locație necunoscută',
          processedDates: processedDates
        });
      });
    });

    this.http.get<any[]>(`https://localhost:44330/api/events/${id}/similar`).subscribe((res: any) => {
      const rawSimilar = res.items || res;
      const mappedSimilar = rawSimilar.map((ev: any) => {
        const matchingVenue = this.venuesList().find((v: any) => v.venueId === ev.venueId);
        return {
          ...ev,
          imageUrl: ev.images?.find((img: any) => img.isBanner)?.url || 'assets/placeholder.jpg',
          venueName: matchingVenue ? matchingVenue.name : 'Locație necunoscută',
          minPrice: ev.minTicketPrice ?? 0
        };
      });
      this.similarEvents.set(mappedSimilar);
    });
  }
}