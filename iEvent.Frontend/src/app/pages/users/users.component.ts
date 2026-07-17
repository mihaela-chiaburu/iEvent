import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { UserService, User } from '../../services/user.service';

@Component({
  standalone: true,
  imports: [FormsModule],
  templateUrl: './users.component.html',
  styleUrl: './users.component.css'
})
export class UsersComponent implements OnInit {
  private userService = inject(UserService);
  
  users = signal<User[]>([]);
  totalCount = signal<number>(0);

  filterSearch = signal<string>('');
  filterRole = signal<string>('');
  filterStatus = signal<string>(''); 

  isCreateModalOpen = false;
  isEditModalOpen = false;

  newManager = { name: '', email: '', password: '', phoneNumber: '', role: 'EventManager' };
  editingUser: { id: string; name: string; email: string; phoneNumber: string } | null = null;

  ngOnInit() { 
    this.loadUsers(); 
  }

loadUsers() {
  let isActiveParam: boolean | undefined = undefined;
  if (this.filterStatus() === 'active') isActiveParam = true;
  if (this.filterStatus() === 'locked') isActiveParam = false;

  this.userService.getUsers({
    search: this.filterSearch(),
    role: this.filterRole(),
    isActive: isActiveParam,
    page: 1,
    pageSize: 50
  }).subscribe({
    next: (res) => {
      const mappedUsers = (res.items || []).map(u => ({
        ...u,
        isActive: u.isActive !== undefined ? u.isActive : true 
      }));
      
      this.users.set(mappedUsers);
      this.totalCount.set(res.totalCount || 0);
    },
    error: () => {
      alert('Eroare la încărcarea utilizatorilor.');
    }
  });
}

toggleLock(user: User) {
  if (user.isActive === false) {
    this.userService.unlockUser(user.id).subscribe({
      next: () => {
        alert('Utilizatorul a fost deblocat!');
        this.users.update(allUsers => 
          allUsers.map(u => u.id === user.id ? { ...u, isActive: true } : u)
        );
      },
      error: (err) => alert('Eroare la deblocare: ' + err.message)
    });
  } else {
    this.userService.lockUser(user.id).subscribe({
      next: () => {
        alert('Utilizatorul a fost blocat!');
        this.users.update(allUsers => 
          allUsers.map(u => u.id === user.id ? { ...u, isActive: false } : u)
        );
      },
      error: (err) => alert('Eroare la blocare: ' + err.message)
    });
  }
}

  applyFilters() {
    this.loadUsers();
  }

  resetFilters() {
    this.filterSearch.set('');
    this.filterRole.set('');
    this.filterStatus.set('');
    this.loadUsers();
  }

  openCreateModal() {
    this.newManager = { name: '', email: '', password: '', phoneNumber: '', role: 'EventManager' };
    this.isCreateModalOpen = true;
  }

  closeCreateModal() {
    this.isCreateModalOpen = false;
  }

  createManagerAccount() {
    if (!this.newManager.email || !this.newManager.password || !this.newManager.name) {
      alert('Te rugăm să completezi numele, email-ul și parola!');
      return;
    }
    
    let roleValue: any = this.newManager.role;
    if (this.newManager.role === 'EventManager') roleValue = 0; 
    if (this.newManager.role === 'BookingManager') roleValue = 1;

    const payload = {
      ...this.newManager,
      role: roleValue
    };

    this.userService.createManager(payload).subscribe({
      next: () => {
        alert('Contul de manager a fost creat cu succes!');
        this.loadUsers();
        this.closeCreateModal();
      },
      error: (err) => {
        alert('Eroare la crearea managerului: ' + (err.error?.message || err.message));
      }
    });
  }

  changeRole(userId: string, newRole: string) {
    if (!userId) return;
    this.userService.updateUserRole(userId, newRole).subscribe({
      next: () => {
        alert('Rolul utilizatorului a fost actualizat!');
        this.loadUsers(); 
      },
      error: () => {
        alert('Eroare la actualizarea rolului.');
      }
    });
  }

  deleteUser(userId: string) {
    if (confirm('Ești sigur că vrei să ștergi acest utilizator?')) {
      this.userService.deleteUser(userId).subscribe({
        next: () => {
          alert('Utilizator șters cu succes!');
          this.loadUsers();
        },
        error: () => {
          alert('Eroare la ștergerea utilizatorului (sau endpoint-ul nu este implementat).');
        }
      });
    }
  }

  openEditModal(user: User) {
    this.editingUser = {
      id: user.id,
      name: user.name || '',
      email: user.email,
      phoneNumber: user.phoneNumber || ''
    };
    this.isEditModalOpen = true;
  }

  closeEditModal() {
    this.isEditModalOpen = false;
    this.editingUser = null;
  }

  saveUserEdit() {
    if (!this.editingUser) return;
    this.userService.updateUser(this.editingUser.id, this.editingUser).subscribe({
      next: () => {
        alert('Datele utilizatorului au fost actualizate!');
        this.loadUsers();
        this.closeEditModal();
      },
      error: () => {
        alert('Eroare la actualizarea datelor.');
      }
    });
  }
}