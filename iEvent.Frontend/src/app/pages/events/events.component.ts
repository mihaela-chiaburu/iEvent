import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { EventItem } from '../../models/event-item.model';
import { SlicePipe } from '@angular/common';
import { EventService } from '../../services/event.service';

@Component({
  standalone: true,
  imports: [RouterLink, SlicePipe],
  templateUrl: './events.component.html',
  styleUrl: './events.component.css'
})
export class EventsComponent implements OnInit {
  private eventService = inject(EventService);
  auth = inject(AuthService);
  events = signal<EventItem[]>([]);

  ngOnInit() {
    this.eventService.getEvents().subscribe(res => this.events.set(res));
  }
}