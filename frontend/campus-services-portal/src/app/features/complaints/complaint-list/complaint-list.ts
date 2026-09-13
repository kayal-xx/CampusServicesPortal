import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
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
  standalone: true,
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
  customCategoryName = '';
  description = '';

  isLoading = false;
  isSubmitting = false;

  errorMessage = '';
  successMessage = '';

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

    if (tab === 'new') {
      this.selectedCategoryId = null;
      this.customCategoryName = '';
      this.description = '';
    }
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
    this.complaintService
      .getActiveCategories()
      .subscribe({
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

  onCategoryChange(): void {
    this.errorMessage = '';

    // If a normal category is selected,
    // clear the custom category field.
    if (!this.isCustomCategory()) {
      this.customCategoryName = '';
    }
  }

  isCustomCategory(): boolean {
    return this.selectedCategoryId === -1;
  }

  submitComplaint(): void {
    const trimmedDescription = this.description.trim();
    const trimmedCustomCategory = this.customCategoryName.trim();

    // Validate category
    if (this.selectedCategoryId === null) {
      this.errorMessage = 'Please select a complaint category.';
      return;
    }

    // Validate custom category
    if (
      this.isCustomCategory() &&
      trimmedCustomCategory.length < 3
    ) {
      this.errorMessage =
        'Please enter a custom category with at least 3 characters.';
      return;
    }

    // Validate description
    if (trimmedDescription.length < 10) {
      this.errorMessage =
        'Complaint description must contain at least 10 characters.';
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.complaintService
      .createComplaint({
        studentId: this.studentId,

        complaintCategoryId:
          this.isCustomCategory()
            ? null
            : this.selectedCategoryId,

        customCategoryName:
          this.isCustomCategory()
            ? trimmedCustomCategory
            : undefined,

        description: trimmedDescription
      })
      .subscribe({
        next: (complaint) => {
          this.complaints = [
            complaint,
            ...this.complaints
          ];

          this.selectedCategoryId = null;
          this.customCategoryName = '';
          this.description = '';

          this.isSubmitting = false;
          this.activeTab = 'list';
          this.successMessage =
            'Complaint submitted successfully.';

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