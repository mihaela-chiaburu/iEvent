import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { VenueService } from '../../services/venue.service'; 
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-create-venue',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './create-venue.component.html',
  styleUrls: ['../create-event/create-event.component.css']
})
export class CreateVenueComponent implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private venueService = inject(VenueService);

  venueId = signal<string | null>(null);
  venueForm!: FormGroup;

  galleryImages = signal<any[]>([]);
  isUploadingGallery = signal<boolean>(false);

  ngOnInit() {
    this.initForm();

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.venueId.set(id);
    } else {
      alert('Id-ul locației lipsește!');
      this.router.navigate(['/venues']);
    }
  }

  initForm() {
    this.venueForm = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      city: ['', Validators.required],
      address: ['', Validators.required],
      latitude: [44.4396, [Validators.required, Validators.min(-90), Validators.max(90)]], 
      longitude: [26.0963, [Validators.required, Validators.min(-180), Validators.max(180)]], 
      capacity: [100, [Validators.required, Validators.min(1)]],
      phone: ['', [Validators.required, Validators.pattern('^[0-9+\\s-]{9,15}$')]],
      email: ['', [Validators.required, Validators.email]],
      facilities: this.fb.array([]) 
    });
  }

  get facilities(): FormArray {
    return this.venueForm.get('facilities') as FormArray;
  }

  addFacility() {
    const facilityGroup = this.fb.group({
      name: ['', Validators.required]
    });
    this.facilities.push(facilityGroup);
  }

  removeFacility(index: number) {
    this.facilities.removeAt(index);
  }

  onGallerySelected(event: any) {
    const files: FileList = event.target.files;
    const currentId = this.venueId();
    if (!files || files.length === 0 || !currentId) return;

    this.isUploadingGallery.set(true);

    this.venueService.uploadVenueImages(currentId, files).subscribe({
      next: (uploadedImages: any[]) => {
        this.galleryImages.update(prev => [...prev, ...uploadedImages]);
        this.isUploadingGallery.set(false);
      },
      error: (err) => {
        console.error(err);
        alert('A apărut o eroare la încărcarea imaginilor.');
        this.isUploadingGallery.set(false);
      }
    });
  }

  publishVenue() {
    if (this.venueForm.invalid) {
      this.venueForm.markAllAsTouched();
      alert('Vă rugăm să completați corect toate câmpurile obligatorii.');
      return;
    }

    const currentId = this.venueId();
    if (!currentId) return;

    const formValues = this.venueForm.value;

    const formattedImages = this.galleryImages().map((img, index) => ({
      url: img.url,
      publicId: img.publicId || '',
      sortOrder: index
    }));

    const patchData = {
      name: formValues.name,
      address: formValues.address,
      city: formValues.city,
      capacity: Number(formValues.capacity),
      latitude: Number(formValues.latitude),
      longitude: Number(formValues.longitude),
      description: formValues.description,
      phone: formValues.phone,
      email: formValues.email,
      facilities: formValues.facilities, 
      images: formattedImages 
    };

    this.venueService.patchVenue(currentId, patchData).subscribe({
      next: () => {
        this.venueService.publishVenue(currentId).subscribe({
          next: () => {
            alert('Locația a fost creată și publicată cu succes!');
            this.router.navigate(['/venues']);
          },
          error: (err) => {
            console.error(err);
            alert('Detaliile s-au salvat, dar a apărut o eroare la publicarea locației.');
          }
        });
      },
      error: (err) => {
        console.error(err);
        alert('Eroare la trimiterea datelor despre locație.');
      }
    });
  }

  deleteDraft() {
    const currentId = this.venueId();
    if (!currentId) return;

    if (confirm('Sigur dorești să anulezi crearea acestei locații? Toate datele vor fi șterse.')) {
      this.venueService.deleteVenue(currentId).subscribe({
        next: () => {
          this.router.navigate(['/venues']);
        },
        error: (err) => {
          console.error(err);
          alert('Eroare la ștergerea draftului.');
        }
      });
    }
  }
}