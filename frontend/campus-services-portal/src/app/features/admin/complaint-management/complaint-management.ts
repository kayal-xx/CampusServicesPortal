import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ComplaintService } from '../../../core/services/complaint.service';
import { ComplaintItem } from '../../../core/models/complaint.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
@Component({
  selector: 'app-complaint-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './complaint-management.html',
  styleUrl: './complaint-management.css'
})
export class ComplaintManagement implements OnInit {
  complaints: ComplaintItem[] = [];

  searchTerm = '';
  selectedStatus = 'All';

  loading = false;
  errorMessage = '';

  selectedComplaint: ComplaintItem | null = null;
  showModal = false;

  updateStatus: 'Pending' | 'In Progress' | 'Resolved' | 'Rejected' = 'Pending';
  resolutionNote = '';
  saving = false;

  constructor(
    private complaintService: ComplaintService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadComplaints();
  }

  loadComplaints(): void {
    this.loading = true;
    this.errorMessage = '';

    const status =
      this.selectedStatus === 'All'
        ? undefined
        : this.selectedStatus;

    this.complaintService.getComplaintsByStatus(status).subscribe({
      next: (data) => {
        this.complaints = Array.isArray(data) ? data : [];
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Failed to load complaints:', error);
        this.complaints = [];
        this.loading = false;
        this.errorMessage = 'Unable to load complaints.';
        this.cdr.detectChanges();
      }
    });
  }

  get filteredComplaints(): ComplaintItem[] {
    const search = this.searchTerm.trim().toLowerCase();

    if (!search) {
      return this.complaints;
    }

    return this.complaints.filter((complaint) =>
      String(complaint.id).includes(search) ||
      String(complaint.studentId).includes(search) ||
      complaint.categoryName.toLowerCase().includes(search) ||
      complaint.description.toLowerCase().includes(search) ||
      complaint.status.toLowerCase().includes(search)
    );
  }

  get pendingCount(): number {
    return this.complaints.filter(
      complaint => complaint.status === 'Pending'
    ).length;
  }

  get inProgressCount(): number {
    return this.complaints.filter(
      complaint => complaint.status === 'In Progress'
    ).length;
  }

  get resolvedCount(): number {
    return this.complaints.filter(
      complaint => complaint.status === 'Resolved'
    ).length;
  }

  get rejectedCount(): number {
    return this.complaints.filter(
      complaint => complaint.status === 'Rejected'
    ).length;
  }

  openUpdateModal(complaint: ComplaintItem): void {
    this.selectedComplaint = complaint;

    this.updateStatus =
      complaint.status as
        | 'Pending'
        | 'In Progress'
        | 'Resolved'
        | 'Rejected';

    this.resolutionNote = complaint.resolutionNote || '';
    this.showModal = true;
  }

  closeModal(): void {
    if (this.saving) {
      return;
    }

    this.showModal = false;
    this.selectedComplaint = null;
    this.resolutionNote = '';
  }

  saveStatus(): void {
    if (!this.selectedComplaint) {
      return;
    }

    if (
      (this.updateStatus === 'Resolved' ||
        this.updateStatus === 'Rejected') &&
      !this.resolutionNote.trim()
    ) {
      this.errorMessage =
        this.updateStatus === 'Rejected'
          ? 'A rejection reason is required.'
          : 'A resolution note is required.';
      return;
    }

    this.saving = true;
    this.errorMessage = '';

    this.complaintService
      .updateComplaintStatus(
        this.selectedComplaint.id,
        this.updateStatus,
        this.resolutionNote.trim()
      )
      .subscribe({
        next: () => {
          this.saving = false;
          this.closeModal();
          this.loadComplaints();
        },
        error: (error) => {
          console.error('Failed to update complaint:', error);

          this.saving = false;
          this.errorMessage =
            error?.error?.message ||
            'Unable to update complaint status.';

          this.cdr.detectChanges();
        }
      });
  }

  getStatusClass(status: string): string {
    return status
      .toLowerCase()
      .replace(/\s+/g, '-');
  }
}