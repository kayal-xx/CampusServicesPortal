import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { finalize } from 'rxjs';

import { Lab, LabBooking } from '../../../core/models/lab.model';
import { LabService } from '../../../core/services/lab';

@Component({
  selector: 'app-lab-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './lab-management.html',
  styleUrl: './lab-management.css'
})
export class LabManagement implements OnInit {
  private readonly labService = inject(LabService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  labs: Lab[] = [];
  bookings: LabBooking[] = [];

  selectedStatus = '';

  loading = false;
  errorMessage = '';
  successMessage = '';

  showLabForm = false;
  editingLabId: number | null = null;

  labName = '';
  labLocation = '';
  labCapacity = 1;
  labIsActive = true;

  rejectionBookingId: number | null = null;
  rejectionReason = '';

  ngOnInit(): void {
    this.loadLabs();
    this.loadBookings();
  }

  loadLabs(): void {
    this.labService.getLabs().subscribe({
      next: (data) => {
        this.labs = data;
        this.changeDetectorRef.detectChanges();
      },
      error: (error: HttpErrorResponse) => {
        console.error('Labs error:', error);
        this.errorMessage =
          error.error?.message ?? 'Unable to load labs.';
      }
    });
  }

  openAddLab(): void {
    this.editingLabId = null;
    this.labName = '';
    this.labLocation = '';
    this.labCapacity = 1;
    this.labIsActive = true;
    this.showLabForm = true;
    this.clearMessages();
  }

  openEditLab(lab: Lab): void {
    this.editingLabId = lab.id;
    this.labName = lab.name;
    this.labLocation = lab.location;
    this.labCapacity = lab.capacity;
    this.labIsActive = lab.isActive;
    this.showLabForm = true;
    this.clearMessages();
  }

  cancelLabForm(): void {
    this.showLabForm = false;
    this.editingLabId = null;
  }

  saveLab(): void {
    if (!this.labName.trim() || !this.labLocation.trim()) {
      this.errorMessage = 'Lab name and location are required.';
      return;
    }

    if (this.labCapacity < 1) {
      this.errorMessage = 'Lab capacity must be at least 1.';
      return;
    }

    const data = {
      name: this.labName.trim(),
      location: this.labLocation.trim(),
      capacity: this.labCapacity,
      isActive: this.labIsActive
    };

    this.loading = true;
    this.clearMessages();

    const request = this.editingLabId
      ? this.labService.updateLab(this.editingLabId, data)
      : this.labService.createLab(data);

    request.subscribe({
      next: () => {
        this.successMessage = this.editingLabId
          ? 'Lab updated successfully.'
          : 'Lab created successfully.';

        this.showLabForm = false;
        this.editingLabId = null;

        this.loadLabs();
        this.loading = false;
      },
      error: (error: HttpErrorResponse) => {
        console.error('Save lab error:', error);

        this.errorMessage =
          error.error?.message ?? 'Unable to save lab.';

        this.loading = false;
        this.changeDetectorRef.detectChanges();
      }
    });
  }

  deactivateLab(lab: Lab): void {
    if (!confirm(`Deactivate ${lab.name}?`)) {
      return;
    }

    this.loading = true;
    this.clearMessages();

    this.labService.deactivateLab(lab.id).subscribe({
      next: () => {
        this.successMessage = 'Lab deactivated successfully.';
        this.loadLabs();
        this.loading = false;
      },
      error: (error: HttpErrorResponse) => {
        console.error('Deactivate lab error:', error);

        this.errorMessage =
          error.error?.message ?? 'Unable to deactivate lab.';

        this.loading = false;
        this.changeDetectorRef.detectChanges();
      }
    });
  }

  loadBookings(): void {
    this.loading = true;
    this.errorMessage = '';

    this.labService
      .getAllBookings(this.selectedStatus || undefined)
      .pipe(
        finalize(() => {
          this.loading = false;
          this.changeDetectorRef.detectChanges();
        })
      )
      .subscribe({
        next: (data) => {
          this.bookings = data;
        },
        error: (error: HttpErrorResponse) => {
          console.error('Lab bookings error:', error);

          this.errorMessage =
            error.error?.message ??
            'Unable to load lab bookings.';
        }
      });
  }

  approveBooking(booking: LabBooking): void {
    this.updateStatus(booking.id, 'Approved');
  }

  startReject(booking: LabBooking): void {
    this.rejectionBookingId = booking.id;
    this.rejectionReason = '';
    this.errorMessage = '';
    this.successMessage = '';
  }

  cancelReject(): void {
    this.rejectionBookingId = null;
    this.rejectionReason = '';
  }

  rejectBooking(booking: LabBooking): void {
    if (!this.rejectionReason.trim()) {
      this.errorMessage = 'Please enter a rejection reason.';
      return;
    }

    this.updateStatus(
      booking.id,
      'Rejected',
      this.rejectionReason.trim()
    );
  }

  private updateStatus(
    bookingId: number,
    status: 'Approved' | 'Rejected',
    rejectionReason = ''
  ): void {
    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.labService
      .updateBookingStatus(
        bookingId,
        status,
        rejectionReason
      )
      .subscribe({
        next: () => {
          this.successMessage =
            status === 'Approved'
              ? 'Lab booking approved successfully.'
              : 'Lab booking rejected successfully.';

          this.rejectionBookingId = null;
          this.rejectionReason = '';

          this.loadBookings();
        },
        error: (error: HttpErrorResponse) => {
          console.error('Update lab booking error:', error);

          this.errorMessage =
            error.error?.message ??
            'Unable to update lab booking status.';

          this.loading = false;
          this.changeDetectorRef.detectChanges();
        }
      });
  }

  getStatusClass(status: string): string {
    return status.toLowerCase();
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }
}

