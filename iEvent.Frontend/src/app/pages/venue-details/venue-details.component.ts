import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { VenueService } from '../../services/venue.service'; 
import { EventService } from '../../services/event.service';

@Component({
  selector: 'app-venue-details',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './venue-details.component.html',
  styleUrl: './venue-details.component.css'
})
export class VenueDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private venueService = inject(VenueService); 
  private eventService = inject(EventService);

  venue = signal<any>(null);
  venueEvents = signal<any[]>([]); 
  isLoading = signal<boolean>(true);

  ngOnInit() {
    const venueId = this.route.snapshot.paramMap.get('id');
    if (venueId) {
      this.loadVenueDetails(venueId);
    }
  }

  loadVenueDetails(id: string) {
    this.isLoading.set(true);

    this.venueService.getVenueById(id).subscribe({
      next: (venueData: any) => {
        if (venueData) {
          this.venue.set(venueData);
          
          this.eventService.getEvents().subscribe({
            next: (response: any) => {
              const eventsArray = response && response.items ? response.items : (Array.isArray(response) ? response : []);
              const filtered = eventsArray.filter((e: any) => e.venueId === id);
              
              this.venueEvents.set(filtered);
              this.isLoading.set(false);
            },
            error: (err) => {
              console.error("Eroare la încărcarea evenimentelor:", err);
              this.isLoading.set(false);
            }
          });
        } else {
          this.isLoading.set(false);
        }
      },
      error: (err) => {
        console.error("Eroare la încărcarea detaliilor locației:", err);
        this.isLoading.set(false);
      }
    });
  }

  getMainImageUrl(venue: any): string {
    if (!venue?.images || venue.images.length === 0) return 'assets/placeholder-venue.jpg';
    const mainImg = venue.images.find((img: any) => img.sortOrder === 0);
    return mainImg ? mainImg.url : venue.images[0].url;
  }
}