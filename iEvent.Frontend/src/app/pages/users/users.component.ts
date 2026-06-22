import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { User } from '../../models/user.model';
import { UserService } from '../../services/user.service';

@Component({
  standalone: true,
  imports: [FormsModule],
  templateUrl: './users.component.html',
  styleUrl: './users.component.css'
})
export class UsersComponent implements OnInit {
  private userService = inject(UserService);
  users = signal<User[]>([]);

  newManager = { email: '', password: '', role: 'EventManager' };

  ngOnInit() { 
    this.loadUsers(); 
  }

  loadUsers() {
    this.userService.getUsers().subscribe({
      next: (res) => {
        this.users.set(res);
      },
      error: () => {
        alert('Error loading users from backend.');
      }
    });
  }

  createManagerAccount() {
    if (!this.newManager.email || !this.newManager.password) {
      alert('Please enter email and password!');
      return;
    }
    this.userService.createManager(this.newManager).subscribe(() => {
      alert('Manager account created successfully!');
      this.loadUsers();
      this.newManager = { email: '', password: '', role: 'EventManager' };
    });
  }

  changeRole(userId: string, newRole: string) {
    if (!userId) return;
    this.userService.updateUserRole(userId, newRole).subscribe(() => {
      alert('User role updated successfully!');
      this.loadUsers(); 
    });
  }
}