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

  getEvents(): Observable<EventItem[]> { 
    return this.http.get<EventItem[]>(this.url); 
  }
  
  getEventById(id: string): Observable<EventItem> { 
    return this.http.get<EventItem>(`${this.url}/${id}`); 
  }
  
  createEvent(data: any): Observable<EventItem> { 
    return this.http.post<EventItem>(this.url, data); 
  }
  
  getEventTicketTypes(id: string): Observable<TicketType[]> { 
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

  createDraft(): Observable<any> {
    return this.http.post<any>(`${this.url}/draft`, {});
  }

  patchEvent(id: string, data: any): Observable<any> {
    return this.http.patch<any>(`${this.url}/${id}`, data);
  }

  uploadBanner(eventId: string, file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<any>(`${this.url}/${eventId}/banner`, formData);
  }

  saveDates(eventId: string, dates: any[]): Observable<any> {
    return this.http.post<any>(`${`${this.url}/${eventId}/dates`}`, dates);
  }

  addTicketType(eventId: string, ticket: any): Observable<any> {
    return this.http.post<any>(`${this.url}/${eventId}/ticket-types`, ticket);
  }

  publishEvent(id: string): Observable<any> {
    return this.http.post<any>(`${this.url}/${id}/publish`, {});
  }

  deleteEvent(id: string): Observable<any> {
    return this.http.delete<any>(`${this.url}/${id}`);
  }

  uploadImages(eventId: string, files: FileList): Observable<any> {
    const formData = new FormData();
    for (let i = 0; i < files.length; i++) {
      formData.append(`Files`, files[i], files[i].name);
    }
    return this.http.post<any>(`${this.url}/${eventId}/images`, formData);
  }

  createTicketType(ticketData: {
    eventId: string;
    name: string;
    price: number;
    quantityAvailable: number; 
    icon?: number;
    availableFrom: string;
    availableUntil: string;   
  }): Observable<any> {
    return this.http.post<any>(`https://localhost:44330/api/ticket-types`, ticketData);
  }
}