import { Component, inject, signal } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { VenueService } from '../../services/venue.service'; // ajusteză calea

@Component({
  selector: 'app-venues-page',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './venues.component.html',
  styleUrls: ['./venues.component.css']
})
export class VenuesComponent {
  private venueService = inject(VenueService);
  private router = inject(Router);

  venues = signal<any[]>([]);
  isLoading = signal<boolean>(false);
  
  isManager = signal<boolean>(true); 

  ngOnInit() {
    this.loadVenues();
  }

  loadVenues() {
    this.isLoading.set(true);
    this.venueService.getVenues().subscribe({
      next: (data) => {
        this.venues.set(data);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  getMainImageUrl(venue: any): string {
    if (venue.images && venue.images.length > 0) {
      return venue.images[0].url;
    }
    return 'assets/images/default-venue.jpg'; 
  }

  goToCreateVenue() {
    this.venueService.createDraft().subscribe({
      next: (draft) => {
        this.router.navigate(['/create-venue', draft.venueId]);
      },
      error: (err) => {
        console.error(err);
        alert('Eroare la inițializarea locației noi.');
      }
    });
  }
}