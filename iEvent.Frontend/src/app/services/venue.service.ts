import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Venue } from '../models/venue.model';

@Injectable({ providedIn: 'root' })
export class VenueService {
  private http = inject(HttpClient);
  private url = 'https://localhost:44330/api/venues';

  getVenues() { 
    return this.http.get<Venue[]>(this.url); 
  }
  
  createVenue(data: any) { 
    return this.http.post(this.url, data); 
  }
}