import { Component, inject } from '@angular/core';
import { AuthService } from '../../services/auth.service';

@Component({
  standalone: true,
  templateUrl: './index.component.html',
  styleUrl: './index.component.css'
})
export class IndexComponent {
  auth = inject(AuthService);
}