import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EventItem } from '../models/event-item.model';
import { TicketType } from '../models/ticket-type.model';

@Injectable({ providedIn: 'root' })
export class EventService {
  private http = inject(HttpClient);
  private url = 'https://localhost:44330/api/events';

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
}