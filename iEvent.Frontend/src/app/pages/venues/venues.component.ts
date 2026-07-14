import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { EventService } from '../../services/event.service';

@Component({
  selector: 'app-venues',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './venues.component.html',
  styleUrl: './venues.component.css'
})
export class VenuesComponent implements OnInit {
  private eventService = inject(EventService);

  venues = signal<any[]>([]);
  isLoading = signal<boolean>(true);

  ngOnInit() {
    this.loadVenues();
  }

  loadVenues() {
    this.eventService.getPopularVenues().subscribe({
      next: (data: any[]) => {
        this.venues.set(data || []);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Eroare la încărcarea locațiilor:', err);
        this.isLoading.set(false);
      }
    });
  }

  getMainImageUrl(venue: any): string {
    if (!venue.images || venue.images.length === 0) {
      return 'assets/placeholder-venue.jpg';
    }
    const mainImg = venue.images.find((img: any) => img.sortOrder === 0);
    return mainImg ? mainImg.url : venue.images[0].url;
  }
}