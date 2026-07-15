import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { EventService } from '../../services/event.service';

@Component({
  selector: 'app-create-event',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './create-event.component.html',
  styleUrl: './create-event.component.css'
})
export class CreateEventComponent implements OnInit {
  private fb = inject(FormBuilder);
  private eventService = inject(EventService);
  private router = inject(Router);

  eventForm!: FormGroup;
  eventId = signal<string | null>(null);
  venues = signal<any[]>([]);
  bannerUrl = signal<string | null>(null);
  isUploadingBanner = signal<boolean>(false);

  categories = [
    'Concerts', 'Teatru', 'Festivaluri', 'Stand-up', 'Copii', 
    'Sport', 'Expoziții', 'Business', 'Parties', 'Filme', 'Altele'
  ];

  ngOnInit() {
    this.initForm();
    this.loadVenues();
    this.startDraft();
  }

  initForm() {
    this.eventForm = this.fb.group({
      name: ['', Validators.required],
      category: [0, Validators.required],
      venueId: ['', Validators.required],
      description: ['', Validators.required],
      eventDates: this.fb.array([]),       
      ticketTypes: this.fb.array([])       
    });
  }

  loadVenues() {
    this.eventService.getPopularVenues().subscribe({
      next: (res) => this.venues.set(res || []),
      error: (err) => console.error('Eroare la încărcarea locațiilor:', err)
    });
  }

  startDraft() {
    this.eventService.createDraft().subscribe({
      next: (res) => {
        this.eventId.set(res.eventId);
      },
      error: (err) => console.error('Nu s-a putut iniția draftul:', err)
    });
  }

  get eventDates() {
    return this.eventForm.get('eventDates') as FormArray;
  }

  addDateGroup() {
    const dateGroup = this.fb.group({
      date: ['', Validators.required],
      timeSlots: this.fb.array([this.createTimeSlot()]) 
    });
    this.eventDates.push(dateGroup);
  }

  removeDateGroup(index: number) {
    this.eventDates.removeAt(index);
  }

  getTimeSlots(dateIndex: number) {
    return this.eventDates.at(dateIndex).get('timeSlots') as FormArray;
  }

  createTimeSlot() {
    return this.fb.group({
      startTime: ['', Validators.required],
      endTime: ['', Validators.required]
    });
  }

  addTimeSlot(dateIndex: number) {
    this.getTimeSlots(dateIndex).push(this.createTimeSlot());
  }

  removeTimeSlot(dateIndex: number, slotIndex: number) {
    this.getTimeSlots(dateIndex).removeAt(slotIndex);
  }

  get ticketTypes() {
    return this.eventForm.get('ticketTypes') as FormArray;
  }

  addTicketType() {
    const ticketGroup = this.fb.group({
      name: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0)]],
      quantity: [10, [Validators.required, Validators.min(1)]],
      availableFrom: ['', Validators.required],
      availableTo: ['', Validators.required]
    });
    this.ticketTypes.push(ticketGroup);
  }

  removeTicketType(index: number) {
    this.ticketTypes.removeAt(index);
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      const currentId = this.eventId();
      
      if (!currentId) {
        alert('Se generează încă ID-ul evenimentului. Încearcă din nou.');
        return;
      }

      this.isUploadingBanner.set(true);
      this.eventService.uploadBanner(currentId, file).subscribe({
        next: (res) => {
          this.bannerUrl.set(res.url);
          this.isUploadingBanner.set(false);
        },
        error: (err) => {
          console.error(err);
          this.isUploadingBanner.set(false);
          alert('Eroare la încărcarea imaginii pe Cloudinary.');
        }
      });
    }
  }

  publishEvent() {
    if (this.eventForm.invalid) {
      this.eventForm.markAllAsTouched();
      alert('Vă rugăm să completați toate câmpurile obligatorii.');
      return;
    }

    const currentId = this.eventId();
    if (!currentId) return;

    const formValues = this.eventForm.value;

    const patchData = {
      name: formValues.name,
      description: formValues.description,
      venueId: formValues.venueId,
      category: Number(formValues.category),
      eventDates: formValues.eventDates 
    };

    this.eventService.patchEvent(currentId, patchData).subscribe({
      next: () => {
        this.eventService.publishEvent(currentId).subscribe({
          next: () => {
            alert('Evenimentul a fost publicat cu succes!');
            this.router.navigate(['/events']);
          },
          error: (err) => {
            console.error(err);
            alert('Eroare la publicarea evenimentului.');
          }
        });
      },
      error: (err) => {
        console.error(err);
        alert('Eroare la salvarea detaliilor evenimentului.');
      }
    });
  }

  deleteDraft() {
    const currentId = this.eventId();
    if (!currentId) {
      this.router.navigate(['/events']);
      return;
    }

    if (confirm('Sigur doriți să ștergeți acest draft? Toate datele introduse se vor pierde.')) {
      this.eventService.deleteEvent(currentId).subscribe({
        next: () => {
          this.router.navigate(['/events']);
        },
        error: (err) => {
          console.error(err);
          this.router.navigate(['/events']);
        }
      });
    }
  }
}