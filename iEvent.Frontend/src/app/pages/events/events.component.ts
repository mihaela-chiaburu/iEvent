import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { EventService } from '../../services/event.service';
import { AuthService } from '../../services/auth.service'; 

@Component({
  selector: 'app-events',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './events.component.html',
  styleUrl: './events.component.css'
})
export class EventsComponent implements OnInit {
  private eventService = inject(EventService);
  auth = inject(AuthService); 

  allEvents = signal<any[]>([]);
  venues = signal<any[]>([]);
  cities = signal<string[]>([]);

  selectedCategory = signal<number | null>(null);
  selectedDate = signal<string>('');
  selectedCity = signal<string>('');
  selectedVenueId = signal<string>('');
  selectedMaxPrice = signal<number | null>(null);
  sortByDate = signal<string>('asc');

  categories = [
    'Concerts', 'Teatru', 'Festivaluri', 'Stand-up', 'Copii', 
    'Sport', 'Expoziții', 'Business', 'Parties', 'Filme', 'Altele'
  ];

  isManager = computed(() => {
    const role = this.auth.currentUser()?.role;
    return role === 'EventManager' || role === 'SuperAdmin';
  });

  filteredEvents = computed(() => {
    let events = [...this.allEvents()];

    if (this.selectedCategory() !== null) {
      events = events.filter(e => e.category === this.selectedCategory());
    }

    if (this.selectedDate()) {
      events = events.filter(e => e.allDates?.some((d: string) => d.startsWith(this.selectedDate())));
    }

    if (this.selectedCity()) {
      events = events.filter(e => e.venue?.city === this.selectedCity());
    }

    if (this.selectedVenueId()) {
      events = events.filter(e => e.venueId === this.selectedVenueId());
    }

    if (this.selectedMaxPrice() !== null) {
      events = events.filter(e => e.minPrice <= (this.selectedMaxPrice() ?? Infinity));
    }

    events.sort((a, b) => {
      const dateA = new Date(a.allDates?.[0] || 0).getTime();
      const dateB = new Date(b.allDates?.[0] || 0).getTime();
      return this.sortByDate() === 'asc' ? dateA - dateB : dateB - dateA;
    });

    return events;
  });

  ngOnInit() {
    this.loadInitialData();
  }

  loadInitialData() {
    this.eventService.getPopularVenues().subscribe((venuesList: any) => {
      this.venues.set(venuesList || []);

      this.eventService.getEvents().subscribe((res: any) => {
        const rawItems = res.items || res; 
        
        const mapped = rawItems.map((ev: any) => {
          const matchingVenue = venuesList?.find((v: any) => v.venueId === ev.venueId);

          return {
            ...ev,
            imageUrl: ev.images?.find((img: any) => img.isBanner)?.url || null,
            venueName: matchingVenue ? matchingVenue.name : 'Locație necunoscută',
            minPrice: ev.minTicketPrice ?? 0
          };
        });
        
        this.allEvents.set(mapped);

        const uniqueCities = [...new Set(venuesList?.map((v: any) => v.city).filter(Boolean))] as string[];
        this.cities.set(uniqueCities);
      });
    });
  }

  selectCategory(index: number | null) {
    this.selectedCategory.set(index);
  }

  onDateChange(event: Event) {
    const val = (event.target as HTMLInputElement).value;
    this.selectedDate.set(val);
  }

  onCityChange(event: Event) {
    const val = (event.target as HTMLSelectElement).value;
    this.selectedCity.set(val);
  }

  onVenueChange(event: Event) {
    const val = (event.target as HTMLSelectElement).value;
    this.selectedVenueId.set(val);
  }

  onPriceChange(event: Event) {
    const val = (event.target as HTMLSelectElement).value;
    this.selectedMaxPrice.set(val ? Number(val) : null);
  }

  onSortChange(event: Event) {
    const val = (event.target as HTMLSelectElement).value;
    this.sortByDate.set(val);
  }

  resetFilters() {
    this.selectedCategory.set(null);
    this.selectedDate.set('');
    this.selectedCity.set('');
    this.selectedVenueId.set('');
    this.selectedMaxPrice.set(null);
    this.sortByDate.set('asc');
  }
}