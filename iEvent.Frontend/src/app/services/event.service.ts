import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { EventItem } from '../models/event-item.model';
import { TicketType } from '../models/ticket-type.model';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class EventService {
  private http = inject(HttpClient);
  private url = 'https://localhost:44330/api/events';
  private venuesUrl = 'https://localhost:44330/api/venues';

  getEvents() { 
    return this.http.get<EventItem[]>(this.url); 
  }
  
  getEventById(id: string) { 
    return this.http.get<EventItem>(`${this.url}/${id}`); 
  }
  
  createEvent(data: any) { 
    return this.http.post<EventItem>(this.url, data); 
  }
  
  getEventTicketTypes(id: string) { 
    return this.http.get<TicketType[]>(`${this.url}/${id}/ticket-types`); 
  }

  getBanners(): Observable<any[]> {
    return this.http.get<any[]>(`${this.url}/banners`);
  }

  getPopularEvents(): Observable<any[]> {
    return this.http.get<any[]>(`${this.url}/popular`);
  }

  getEventsByCategory(categoryIndex: number): Observable<any> {
    const params = new HttpParams().set('Category', categoryIndex.toString());
    return this.http.get<any>(this.url, { params });
  }

  getEventsByDate(dateStr: string): Observable<any> {
    const params = new HttpParams()
      .set('FromDate', dateStr)
      .set('ToDate', dateStr);
    return this.http.get<any>(this.url, { params });
  }

  getPopularVenues(): Observable<any[]> {
    return this.http.get<any[]>(`${this.venuesUrl}/popular`);
  }
}