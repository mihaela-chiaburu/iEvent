import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Venue } from '../models/venue.model';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class VenueService {
  private http = inject(HttpClient);
  private url = 'https://localhost:44330/api/venues';

  getVenues() { 
    return this.http.get<Venue[]>(this.url); 
  }
  
  getVenueById(id: string): Observable<Venue> {
    return this.http.get<Venue>(`${this.url}/${id}`);
  }
  
  createVenue(data: any) { 
    return this.http.post(this.url, data); 
  }
}