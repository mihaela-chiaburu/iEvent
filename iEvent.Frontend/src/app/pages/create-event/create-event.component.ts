import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { EventService } from '../../services/event.service';
import { forkJoin, of } from 'rxjs'; 

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
  galleryImages = signal<any[]>([]); 
  isUploadingBanner = signal<boolean>(false);
  isUploadingGallery = signal<boolean>(false);

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

  onBannerSelected(event: Event) {
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
          alert('Eroare la încărcarea bannerului.');
        }
      });
    }
  }

  onGallerySelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const files = input.files;
      const currentId = this.eventId();

      if (!currentId) {
        alert('Se generează încă ID-ul evenimentului. Încearcă din nou.');
        return;
      }

      this.isUploadingGallery.set(true);
      this.eventService.uploadImages(currentId, files).subscribe({
        next: (res) => {
          this.galleryImages.set([...this.galleryImages(), ...res]);
          this.isUploadingGallery.set(false);
        },
        error: (err) => {
          console.error(err);
          this.isUploadingGallery.set(false);
          alert('Eroare la încărcarea imaginilor în galerie.');
        }
      });
    }
  }

  publishEvent() {
    if (this.eventForm.invalid) {
      this.eventForm.markAllAsTouched();
      alert('Vă rugăm să completați toate câmpurile obligatorii din formular.');
      return;
    }

    const currentId = this.eventId();
    if (!currentId) return;

    const formValues = this.eventForm.value;

    const currentImages: any[] = [];

    if (this.bannerUrl()) {
      currentImages.push({
        url: this.bannerUrl(),
        publicId: '', 
        sortOrder: 0,
        isBanner: true
      });
    }

    if (this.galleryImages() && this.galleryImages().length > 0) {
      this.galleryImages().forEach((img, index) => {
        currentImages.push({
          url: img.url,
          publicId: img.publicId || '',
          sortOrder: index + 1,
          isBanner: false
        });
      });
    }

    const patchData = {
      name: formValues.name,
      description: formValues.description,
      venueId: formValues.venueId,
      category: Number(formValues.category),
      images: currentImages 
    };

    this.eventService.patchEvent(currentId, patchData).subscribe({
      next: () => {
        const dateRequests = formValues.eventDates.length > 0 
          ? this.eventService.saveDates(currentId, formValues.eventDates) 
          : of(null);

        const ticketRequests = formValues.ticketTypes.map((ticket: any) => {
          const ticketPayload = {
            eventId: currentId,
            name: ticket.name,
            price: ticket.price,
            quantityAvailable: ticket.quantity || 0, 
            icon: 0, 
            availableFrom: new Date(ticket.availableFrom).toISOString(),
            availableUntil: new Date(ticket.availableTo).toISOString() 
          };
          
          return this.eventService.createTicketType(ticketPayload);
        });

        const allTicketsRequest = ticketRequests.length > 0 
          ? forkJoin(ticketRequests) 
          : of(null);

        forkJoin([dateRequests, allTicketsRequest]).subscribe({
          next: () => {
            this.eventService.publishEvent(currentId).subscribe({
              next: () => {
                alert('Evenimentul a fost publicat cu succes!');
                this.router.navigate(['/events']);
              },
              error: (err) => {
                console.error(err);
                alert('Detaliile s-au salvat, dar a apărut o eroare la publicarea evenimentului.');
              }
            });
          },
          error: (err) => {
            console.error(err);
            alert('Eroare la salvarea biletelor sau a datelor calendaristice.');
          }
        });

      },
      error: (err) => {
        console.error(err);
        alert('Eroare la salvarea datelor de bază ale evenimentului.');
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

  saveEventDetails(eventId: string) {
    const formValues = this.eventForm.value;

    if (formValues.eventDates && formValues.eventDates.length > 0) {
      this.eventService.saveDates(eventId, formValues.eventDates).subscribe({
        next: (res) => console.log('Datele calendaristice au fost salvate cu succes!', res),
        error: (err) => console.error('Eroare la salvarea datelor calendaristice:', err)
      });
    }

    if (formValues.ticketTypes && formValues.ticketTypes.length > 0) {
      formValues.ticketTypes.forEach((ticket: any) => {
        
        const ticketPayload = {
          eventId: eventId,
          name: ticket.name,
          price: ticket.price,
          quantityAvailable: ticket.quantity || 0, 
          icon: 0, 
          availableFrom: new Date(ticket.availableFrom).toISOString(),
          availableUntil: new Date(ticket.availableTo).toISOString() 
        };

        this.eventService.createTicketType(ticketPayload).subscribe({
          next: (res) => {
            console.log(`Biletul "${ticket.name}" a fost creat cu succes!`, res);
          },
          error: (err) => {
            console.error(`Eroare la salvarea biletului "${ticket.name}":`, err);
          }
        });
      });
    }
  }
}