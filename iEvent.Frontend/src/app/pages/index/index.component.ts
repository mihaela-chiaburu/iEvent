import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { EventService } from '../../services/event.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-events',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './index.component.html',
  styleUrl: './index.component.css'
})
export class IndexComponent implements OnInit {
  private eventService = inject(EventService);
  auth = inject(AuthService);

  banners = signal<any[]>([]);
  popularEvents = signal<any[]>([]);
  categoryEvents = signal<any[]>([]);
  dateEvents = signal<any[]>([]);
  popularVenues = signal<any[]>([]);

  categories = [
    'Concerts', 'Teatru', 'Festivaluri', 'Stand-up', 'Copii', 
    'Sport', 'Expoziții', 'Business', 'Parties', 'Filme', 'Altele'
  ];
  selectedCategory = signal<number>(0);

  availableDates = signal<string[]>([]);
  selectedDate = signal<string>('');

  ngOnInit() {
    this.loadInitialData();
    this.generateFilterDates();
  }

loadInitialData() {
  this.eventService.getBanners().subscribe(res => {
    const mapped = res.map(ev => ({
      ...ev,
      imageUrl: ev.imageUrl || ev.images?.find((img: any) => img.isBanner)?.url
    }));
    this.banners.set(mapped);
  });

  this.eventService.getPopularEvents().subscribe(res => {
    const mapped = res.map(ev => ({
      ...ev,
      imageUrl: ev.images?.find((img: any) => img.isBanner)?.url
    }));
    this.popularEvents.set(mapped);
  });

  this.selectCategory(0);

  this.eventService.getPopularVenues().subscribe(res => {
    const mapped = res.map(v => {
      const primaryImage = v.images?.find((img: any) => img.sortOrder === 0);
      return {
        ...v,
        venueImageUrl: primaryImage ? primaryImage.url : (v.images?.[0]?.url || null)
      };
    });
    this.popularVenues.set(mapped);
  });
}

  selectCategory(index: number) {
    this.selectedCategory.set(index);
    this.eventService.getEventsByCategory(index).subscribe(res => {
      this.categoryEvents.set(res.items || []);
    });
  }

  generateFilterDates() {
    const dates: string[] = [];
    const today = new Date();
    
    for (let i = 0; i < 7; i++) {
      const nextDate = new Date(today);
      nextDate.setDate(today.getDate() + i);
      const formatted = nextDate.toISOString().split('T')[0];
      dates.push(formatted);
    }
    
    this.availableDates.set(dates);
    if (dates.length > 0) {
      this.selectDate(dates[0]); 
    }
  }

  selectDate(dateStr: string) {
    this.selectedDate.set(dateStr);
    this.eventService.getEventsByDate(dateStr).subscribe(res => {
      this.dateEvents.set(res.items || []);
    });
  }
}