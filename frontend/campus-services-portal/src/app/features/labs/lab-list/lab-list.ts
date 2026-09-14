import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { finalize } from 'rxjs';

import {
  CreateLabBooking,
  Lab,
  LabBooking
} from '../../../core/models/lab.model';
import { LabService } from '../../../core/services/lab';
import { Navbar } from '../../../shared/navbar/navbar';

@Component({
  selector: 'app-lab-list',
  imports: [
    CommonModule,
    FormsModule,
    Navbar
  ],
  templateUrl: './lab-list.html',
  styleUrl: './lab-list.css'
})
export class LabList implements OnInit {
  labs: Lab[] = [];
  bookings: LabBooking[] = [];

  activeTab: 'labs' | 'book' | 'bookings' = 'labs';

  selectedLabId: number | null = null;
  bookingDate = '';
  startTime = '';
  endTime = '';

  isLoading = false;
  isSubmitting = false;

  errorMessage = '';
  successMessage = '';

  readonly minimumDate = new Date()
    .toISOString()
    .split('T')[0];

  constructor(
    private readonly labService: LabService,
    private readonly changeDetector: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    this.clearMessages();

    this.labService.getLabs()
      .pipe(
        finalize(() => {
          this.isLoading = false;
          this.changeDetector.markForCheck();
        })
      )
      .subscribe({
        next: (labs) => {
          this.labs = labs;
        },
        error: (error: HttpErrorResponse) => {
          this.errorMessage = this.getErrorMessage(
            error,
            'Unable to load labs.'
          );
        }
      });

    this.labService.getMyBookings().subscribe({
      next: (bookings) => {
        this.bookings = bookings;
        this.changeDetector.markForCheck();
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage = this.getErrorMessage(
          error,
          'Unable to load your lab bookings.'
        );
        this.changeDetector.markForCheck();
      }
    });
  }

  showLabs(): void {
    this.clearMessages();
    this.activeTab = 'labs';
  }

  openBooking(labId?: number): void {
    if (labId !== undefined) {
      this.selectedLabId = labId;
    }

    this.clearMessages();
    this.activeTab = 'book';
  }

  showBookings(): void {
    this.clearMessages();
    this.activeTab = 'bookings';
  }

  submitBooking(): void {
    this.clearMessages();

    if (
      !this.selectedLabId ||
      !this.bookingDate ||
      !this.startTime ||
      !this.endTime
    ) {
      this.errorMessage =
        'Please complete all booking fields.';
      return;
    }

    if (this.startTime >= this.endTime) {
      this.errorMessage =
        'End time must be later than start time.';
      return;
    }

    const request: CreateLabBooking = {
      labId: this.selectedLabId,
      bookingDate: this.bookingDate,
      startTime: this.formatTime(this.startTime),
      endTime: this.formatTime(this.endTime)
    };

    this.isSubmitting = true;

    this.labService.createBooking(request)
      .pipe(
        finalize(() => {
          this.isSubmitting = false;
          this.changeDetector.markForCheck();
        })
      )
      .subscribe({
        next: (booking) => {
          this.bookings = [
            booking,
            ...this.bookings
          ];

          this.successMessage =
            'Lab reservation created successfully.';

          this.resetForm();
          this.activeTab = 'bookings';
        },
        error: (error: HttpErrorResponse) => {
          this.errorMessage = this.getErrorMessage(
            error,
            'Unable to create the lab reservation.'
          );
        }
      });
  }

  cancelBooking(booking: LabBooking): void {
    if (booking.status === 'Cancelled') {
      return;
    }

    const confirmed = window.confirm(
      `Cancel the reservation for ${booking.labName}?`
    );

    if (!confirmed) {
      return;
    }

    this.clearMessages();

    this.labService.cancelBooking(booking.id)
      .subscribe({
        next: (response) => {
          booking.status = 'Cancelled';
          this.successMessage = response.message;
          this.changeDetector.markForCheck();
        },
        error: (error: HttpErrorResponse) => {
          this.errorMessage = this.getErrorMessage(
            error,
            'Unable to cancel the reservation.'
          );
          this.changeDetector.markForCheck();
        }
      });
  }

  getSelectedLab(): Lab | undefined {
    return this.labs.find(
      (lab) => lab.id === this.selectedLabId
    );
  }

  private formatTime(time: string): string {
    return time.length === 5
      ? `${time}:00`
      : time;
  }

  private resetForm(): void {
    this.selectedLabId = null;
    this.bookingDate = '';
    this.startTime = '';
    this.endTime = '';
  }

  private clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  private getErrorMessage(
    error: HttpErrorResponse,
    fallbackMessage: string
  ): string {
    return error.error?.message ?? fallbackMessage;
  }
}