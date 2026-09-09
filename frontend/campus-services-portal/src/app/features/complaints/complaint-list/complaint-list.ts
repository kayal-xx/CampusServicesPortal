import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';
import { FormsModule } from '@angular/forms';

import {
  ComplaintCategory,
  ComplaintItem
} from '../../../core/models/complaint.model';
import { ComplaintService } from '../../../core/services/complaint.service';
import { Auth } from '../../../core/services/auth';
import { Navbar } from '../../../shared/navbar/navbar';
type ComplaintTab = 'list' | 'new';

@Component({
  selector: 'app-complaint-list',
 imports: [
  CommonModule,
  FormsModule,
  Navbar
],
  templateUrl: './complaint-list.html',
  styleUrl: './complaint-list.css'
})
export class ComplaintList implements OnInit {
  complaints: ComplaintItem[] = [];
  categories: ComplaintCategory[] = [];

  activeTab: ComplaintTab = 'list';
  selectedComplaint: ComplaintItem | null = null;

  selectedCategoryId: number | null = null;
  description = '';

  isLoading = false;
  isSubmitting = false;

  errorMessage = '';
  successMessage = '';

  // Replace this with the authenticated student ID later.
  studentId = 0;

 constructor(
  private complaintService: ComplaintService,
  private changeDetectorRef: ChangeDetectorRef,
  private authService: Auth
) {}

 ngOnInit(): void {
  const currentUser = this.authService.getCurrentUser();

  if (!currentUser) {
    this.errorMessage = 'Please sign in again.';
    return;
  }

  this.studentId = currentUser.studentId;

  this.loadComplaints();
  this.loadCategories();
}

  setTab(tab: ComplaintTab): void {
    this.activeTab = tab;
    this.errorMessage = '';
    this.successMessage = '';
  }

  loadComplaints(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.complaintService
      .getStudentComplaints(this.studentId)
      .subscribe({
        next: (complaints) => {
          this.complaints = complaints;
          this.isLoading = false;
          this.changeDetectorRef.detectChanges();
        },
        error: (error) => {
          this.errorMessage =
            error.error?.message ??
            'Unable to load complaints. Please try again.';
          this.isLoading = false;
          this.changeDetectorRef.detectChanges();
        }
      });
  }

  loadCategories(): void {
    this.complaintService.getActiveCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
        this.changeDetectorRef.detectChanges();
      },
      error: () => {
        this.categories = [];
        this.changeDetectorRef.detectChanges();
      }
    });
  }

  submitComplaint(): void {
    const trimmedDescription = this.description.trim();

    if (!this.selectedCategoryId) {
      this.errorMessage = 'Please select a complaint category.';
      return;
    }

    if (trimmedDescription.length < 10) {
      this.errorMessage =
        'Complaint description must contain at least 10 characters.';
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.complaintService.createComplaint({
      studentId: this.studentId,
      complaintCategoryId: this.selectedCategoryId,
      description: trimmedDescription
    }).subscribe({
      next: (complaint) => {
        this.complaints = [complaint, ...this.complaints];
        this.selectedCategoryId = null;
        this.description = '';
        this.isSubmitting = false;
        this.activeTab = 'list';
        this.successMessage = 'Complaint submitted successfully.';
        this.changeDetectorRef.detectChanges();
      },
      error: (error) => {
        this.errorMessage =
          error.error?.message ??
          'Unable to submit the complaint. Please try again.';
        this.isSubmitting = false;
        this.changeDetectorRef.detectChanges();
      }
    });
  }

  openDetails(complaint: ComplaintItem): void {
    this.selectedComplaint = complaint;
  }

  closeDetails(): void {
    this.selectedComplaint = null;
  }

  getStatusClass(status: string): string {
    return status
      .trim()
      .toLowerCase()
      .replace(/\s+/g, '-');
  }
}