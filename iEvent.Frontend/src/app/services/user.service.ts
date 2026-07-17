import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface User {
  id: string;
  customerId?: string;
  adminId?: string;
  email: string;
  name: string;
  phoneNumber?: string;
  roles: string[];
  isActive?: boolean;
  createdAt?: string; 
}

export interface PaginatedUsers {
  items: User[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

@Injectable({ providedIn: 'root' })
export class UserService {
  private http = inject(HttpClient);
  private url = 'https://localhost:44330/api/users';

  getUsers(filters: { search?: string; role?: string; isActive?: boolean; page?: number; pageSize?: number } = {}): Observable<PaginatedUsers> {
    let params = new HttpParams();
    
    if (filters.search) {
      params = params.set('Search', filters.search);
    }
    if (filters.role) {
      params = params.set('Role', filters.role);
    }
    if (filters.isActive !== undefined) {
      params = params.set('IsActive', filters.isActive.toString());
    }
    if (filters.page) {
      params = params.set('Page', filters.page.toString());
    }
    if (filters.pageSize) {
      params = params.set('PageSize', filters.pageSize.toString());
    }

    return this.http.get<PaginatedUsers>(this.url, { params });
  }
  
  createManager(data: any): Observable<any> { 
    return this.http.post(`${this.url}/create-manager`, data); 
  }
  
  updateUserRole(id: string, role: string): Observable<any> { 
    return this.http.put(`${this.url}/${id}/role`, { role }); 
  }

  lockUser(id: string): Observable<any> {
    return this.http.patch(`${this.url}/${id}/lock`, {});
  }

  unlockUser(id: string): Observable<any> {
    return this.http.patch(`${this.url}/${id}/unlock`, {});
  }

  deleteUser(id: string): Observable<any> {
    return this.http.delete(`${this.url}/${id}`);
  }

  updateUser(id: string, data: any): Observable<any> {
    return this.http.put(`${this.url}/${id}`, data);
  }
}