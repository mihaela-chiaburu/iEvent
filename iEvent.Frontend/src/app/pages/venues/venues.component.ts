import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Venue } from '../../models/venue.model';
import { VenueService } from '../../services/venue.service';

@Component({
  standalone: true,
  imports: [FormsModule],
  templateUrl: './venues.component.html',
  styleUrl: './venues.component.css'
})
export class VenuesComponent implements OnInit {
  private venueService = inject(VenueService);
  auth = inject(AuthService);
  venues = signal<Venue[]>([]);

  newVenue = { name: '', city: '', address: '', capacity: 0, latitude: 0, longitude: 0 };

  ngOnInit() { this.loadVenues(); }

  loadVenues() { this.venueService.getVenues().subscribe(res => this.venues.set(res)); }

  addVenue() {
    this.venueService.createVenue(this.newVenue).subscribe(() => {
      alert('Venue added successfully!');
      this.loadVenues();
      this.newVenue = { name: '', city: '', address: '', capacity: 0, latitude: 0, longitude: 0 };
    });
  }
}