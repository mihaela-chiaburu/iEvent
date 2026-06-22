import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Booking } from '../models/booking.model';

@Injectable({ providedIn: 'root' })
export class BookingService {
  private http = inject(HttpClient);
  private url = 'https://localhost:44330/api/bookings';

  getMyBookings() { 
    return this.http.get<Booking[]>(`${this.url}/my`); 
  }
  
  getAllBookings() { 
    return this.http.get<Booking[]>(this.url); 
  }
  
  getBookingById(id: string) { 
    return this.http.get<Booking>(`${this.url}/${id}`); 
  }
  
  createBooking(data: any) { 
    return this.http.post(this.url, data); 
  }
  
  updateBooking(id: string, data: any) { 
    return this.http.put(`${this.url}/${id}`, data); 
  }
  
  deleteBooking(id: string) { 
    return this.http.delete(`${this.url}/${id}`); 
  }
  
  markPaid(id: string) { 
    return this.http.patch(`${this.url}/${id}/mark-paid`, {}); 
  }
  
  markUnpaid(id: string) { 
    return this.http.patch(`${this.url}/${id}/mark-unpaid`, {}); 
  }
}