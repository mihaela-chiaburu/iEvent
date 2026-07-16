import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Booking } from '../models/booking.model';

@Injectable({ providedIn: 'root' })
export class BookingService {
  private http = inject(HttpClient);
  private url = 'https://localhost:44330/api/bookings';

  getMyBookings(): Observable<Booking[]> { 
    return this.http.get<Booking[]>(`${this.url}/my`); 
  }
  
  getAllBookings(): Observable<Booking[]> { 
    return this.http.get<Booking[]>(this.url); 
  }

  getBookingById(id: string): Observable<Booking> { 
    return this.http.get<Booking>(`${this.url}/${id}`); 
  }

  getBookingByCode(code: string): Observable<any> {
    return this.http.get<any>(`${this.url}/code/${code}`);
  }
  
  createBooking(data: any): Observable<any> { 
    return this.http.post<any>(this.url, data); 
  }
  
  updateBooking(id: string, data: any): Observable<any> { 
    return this.http.put<any>(`${this.url}/${id}`, data); 
  }

  updateBookingStatus(id: string, status: number): Observable<any> {
    return this.http.put<any>(`${this.url}/${id}`, { status });
  }
  
  deleteBooking(id: string): Observable<any> { 
    return this.http.delete<any>(`${this.url}/${id}`); 
  }

  markPaid(id: string): Observable<any> { 
    return this.http.patch<any>(`${this.url}/${id}/mark-paid`, {}); 
  }
  
  markUnpaid(id: string): Observable<any> { 
    return this.http.patch<any>(`${this.url}/${id}/mark-unpaid`, {}); 
  }

  cancelBooking(id: string): Observable<any> {
    return this.http.patch<any>(`${this.url}/${id}/cancel`, {});
  }

  simulatePayment(id: string, shouldSucceed: boolean = true): Observable<any> {
    return this.http.post<any>(`${this.url}/${id}/simulate-payment`, { shouldSucceed });
  }

  collectAtVenue(id: string, collectedAmount: number): Observable<any> {
    return this.http.patch<any>(`${this.url}/${id}/collect-at-venue`, { 
      collectedAt: new Date().toISOString(), 
      amount: collectedAmount 
    });
  }

  getQr(id: string): Observable<any> {
    return this.http.get<any>(`${this.url}/${id}/qr`);
  }

  getBookingQr(id: string): Observable<any> {
    return this.getQr(id); 
  }

  downloadPdf(id: string): Observable<Blob> {
    return this.http.get(`${this.url}/${id}/pdf`, {
      responseType: 'blob'
    });
  }

  getBookings(filters: {
    status?: number;
    dateFrom?: string;
    dateTo?: string;
    search?: string;
    page?: number;
    pageSize?: number;
  }): Observable<any> {
    let params = new HttpParams();
    
    if (filters.status !== undefined && filters.status !== null && filters.status !== -1) {
      params = params.set('Status', filters.status.toString());
    }
    if (filters.dateFrom) {
      params = params.set('DateFrom', filters.dateFrom);
    }
    if (filters.dateTo) {
      params = params.set('DateTo', filters.dateTo);
    }
    if (filters.search) {
      params = params.set('Search', filters.search);
    }
    if (filters.page !== undefined) {
      params = params.set('Page', filters.page.toString());
    }
    if (filters.pageSize !== undefined) {
      params = params.set('PageSize', filters.pageSize.toString());
    }

    return this.http.get<any>(this.url, { params });
  }

  createBookingByManager(payload: any): Observable<any> {
    return this.http.post<any>(`${this.url}/by-manager`, payload);
  }
}