import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TicketType } from '../models/ticket-type.model';

@Injectable({ providedIn: 'root' })
export class TicketTypeService {
  private http = inject(HttpClient);
  private url = 'https://localhost:44330/api/ticket-types';

  getTicketTypes() { 
    return this.http.get<TicketType[]>(this.url); 
  }
  
  createTicketType(data: any) { 
    return this.http.post(this.url, data); 
  }
  
  updateTicketType(id: string, data: any) { 
    return this.http.put(`${this.url}/${id}`, data); 
  }
}