import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class VenueService {
  private http = inject(HttpClient);
  private venuesUrl = 'https://localhost:44330/api/venues';

  getVenues(): Observable<any[]> {
    return this.http.get<any[]>(this.venuesUrl);
  }

  getVenueById(id: string): Observable<any> {
    return this.http.get<any>(`${this.venuesUrl}/${id}`);
  }

  createDraft(): Observable<any> {
    return this.http.post<any>(`${this.venuesUrl}/draft`, {});
  }

  patchVenue(id: string, data: any): Observable<any> {
    return this.http.patch<any>(`${this.venuesUrl}/${id}`, data);
  }

  uploadVenueImages(venueId: string, files: FileList): Observable<any> {
    const formData = new FormData();
    for (let i = 0; i < files.length; i++) {
      formData.append(`Files`, files[i], files[i].name);
    }
    return this.http.post<any>(`${this.venuesUrl}/${venueId}/images`, formData);
  }

  publishVenue(id: string): Observable<any> {
    return this.http.post<any>(`${this.venuesUrl}/${id}/publish`, {});
  }

  deleteVenue(id: string): Observable<any> {
    return this.http.delete<any>(`${this.venuesUrl}/${id}`);
  }
}