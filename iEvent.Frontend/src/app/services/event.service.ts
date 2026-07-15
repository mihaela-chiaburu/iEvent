import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { EventItem } from '../models/event-item.model';
import { TicketType } from '../models/ticket-type.model';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class EventService {
  private http = inject(HttpClient);
  // Folosim proprietatea ta de bază definită la început
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

  // Obține locațiile populare (folosește URL-ul corect definit de tine)
  getPopularVenues(): Observable<any[]> {
    return this.http.get<any[]>(`${this.venuesUrl}/popular`);
  }

  // --- Metode pentru fluxul de Event Manager (Draft -> Patch -> Publish) ---

  // Creează un draft gol și returnează datele lui (inclusiv eventId primit de la backend)
  createDraft(): Observable<any> {
    return this.http.post<any>(`${this.url}/draft`, {});
  }

  // Actualizează detaliile generale ale evenimentului draftat
  patchEvent(id: string, data: any): Observable<any> {
    return this.http.patch<any>(`${this.url}/${id}`, data);
  }

  // Încarcă bannerul în Cloudinary pentru evenimentul dat
  uploadBanner(eventId: string, file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<any>(`${this.url}/${eventId}/banner`, formData);
  }

  // Adaugă/actualizează datele evenimentului
  saveDates(eventId: string, dates: any[]): Observable<any> {
    return this.http.post<any>(`${`${this.url}/${eventId}/dates`}`, dates);
  }

  // Adaugă tipurile de bilet la eveniment
  addTicketType(eventId: string, ticket: any): Observable<any> {
    return this.http.post<any>(`${this.url}/${eventId}/ticket-types`, ticket);
  }

  // Publică evenimentul de la stadiul de draft la activ
  publishEvent(id: string): Observable<any> {
    return this.http.post<any>(`${this.url}/${id}/publish`, {});
  }

  // Șterge draft-ul sau evenimentul definitiv
  deleteEvent(id: string): Observable<any> {
    return this.http.delete<any>(`${this.url}/${id}`);
  }
}