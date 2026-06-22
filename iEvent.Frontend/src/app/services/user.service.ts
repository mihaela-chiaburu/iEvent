import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class UserService {
  private http = inject(HttpClient);
  private url = 'https://localhost:44330/api/users';

  getUsers() { 
    return this.http.get<User[]>(this.url); 
  }
  
  createManager(data: any) { 
    return this.http.post(`${this.url}/create-manager`, data); 
  }
  
  updateUserRole(id: string, role: string) { 
    return this.http.put(`${this.url}/${id}/role`, { role }); 
  }
}