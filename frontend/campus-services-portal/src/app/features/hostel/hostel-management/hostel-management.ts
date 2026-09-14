import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';

import {
  Hostel,
  HostelApplication,
  HostelRoom
} from '../../../core/models/hostel.model';

import { HostelService } from '../../../core/services/hostel';

@Component({
  selector: 'app-hostel-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './hostel-management.html',
  styleUrl: './hostel-management.css'
})
export class HostelManagement implements OnInit {
  private readonly hostelService = inject(HostelService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  hostels: Hostel[] = [];
  rooms: HostelRoom[] = [];
  applications: HostelApplication[] = [];

  selectedHostelId: number | null = null;
  selectedStatus = '';

  loading = false;
  errorMessage = '';
  successMessage = '';

  showHostelForm = false;
  editingHostelId: number | null = null;

  hostelName = '';
  hostelLocation = '';

  showRoomForm = false;
  editingRoomId: number | null = null;

  roomHostelId: number | null = null;
  roomNumber = '';
  roomCapacity = 1;

  rejectionApplicationId: number | null = null;
  rejectionReason = '';

  ngOnInit(): void {
    this.loadHostels();
    this.loadApplications();
  }

  loadHostels(): void {
    this.loading = true;
    this.errorMessage = '';

    this.hostelService.getHostels().subscribe({
      next: (data) => {
        this.hostels = data;
        this.loading = false;
        this.changeDetectorRef.detectChanges();
      },
      error: (error) => {
        console.error('Hostels error:', error);
        this.errorMessage = 'Unable to load hostels.';
        this.loading = false;
      }
    });
  }

  loadApplications(): void {
    this.hostelService
      .getAllApplications(this.selectedStatus || undefined)
      .subscribe({
        next: (data) => {
          this.applications = data;
          this.changeDetectorRef.detectChanges();
        },
        error: (error) => {
          console.error('Applications error:', error);
          this.errorMessage = 'Unable to load hostel applications.';
        }
      });
  }

  openAddHostel(): void {
    this.editingHostelId = null;
    this.hostelName = '';
    this.hostelLocation = '';
    this.showHostelForm = true;
    this.clearMessages();
  }

  openEditHostel(hostel: Hostel): void {
    this.editingHostelId = hostel.id;
    this.hostelName = hostel.name;
    this.hostelLocation = hostel.location;
    this.showHostelForm = true;
    this.clearMessages();
  }

  cancelHostelForm(): void {
    this.showHostelForm = false;
    this.editingHostelId = null;
  }

  saveHostel(): void {
    if (!this.hostelName.trim() || !this.hostelLocation.trim()) {
      this.errorMessage = 'Hostel name and location are required.';
      return;
    }

    const data = {
      name: this.hostelName.trim(),
      location: this.hostelLocation.trim()
    };

    this.loading = true;
    this.clearMessages();

    const request = this.editingHostelId
      ? this.hostelService.updateHostel(this.editingHostelId, data)
      : this.hostelService.createHostel(data);

    request.subscribe({
      next: () => {
        this.successMessage = this.editingHostelId
          ? 'Hostel updated successfully.'
          : 'Hostel created successfully.';

        this.showHostelForm = false;
        this.editingHostelId = null;
        this.loadHostels();
      },
      error: (error) => {
        console.error('Save hostel error:', error);
        this.errorMessage =
          error?.error?.message || 'Unable to save hostel.';
        this.loading = false;
      }
    });
  }

  deactivateHostel(hostel: Hostel): void {
    if (!confirm(`Deactivate ${hostel.name}?`)) {
      return;
    }

    this.loading = true;
    this.clearMessages();

    this.hostelService.deleteHostel(hostel.id).subscribe({
      next: () => {
        this.successMessage = 'Hostel deactivated successfully.';
        this.loadHostels();
      },
      error: (error) => {
        console.error('Deactivate hostel error:', error);
        this.errorMessage =
          error?.error?.message || 'Unable to deactivate hostel.';
        this.loading = false;
      }
    });
  }

  selectHostel(hostelId: number): void {
    this.selectedHostelId = hostelId;
    this.loadRooms(hostelId);
  }

  loadRooms(hostelId: number): void {
    this.hostelService.getRooms(hostelId).subscribe({
      next: (data) => {
        this.rooms = data;
        this.changeDetectorRef.detectChanges();
      },
      error: (error) => {
        console.error('Rooms error:', error);
        this.errorMessage = 'Unable to load rooms.';
      }
    });
  }

  openAddRoom(hostelId: number): void {
    this.editingRoomId = null;
    this.roomHostelId = hostelId;
    this.roomNumber = '';
    this.roomCapacity = 1;
    this.showRoomForm = true;
    this.clearMessages();
  }

  openEditRoom(room: HostelRoom): void {
    this.editingRoomId = room.id;
    this.roomHostelId = room.hostelId;
    this.roomNumber = room.roomNumber;
    this.roomCapacity = room.capacity;
    this.showRoomForm = true;
    this.clearMessages();
  }

  cancelRoomForm(): void {
    this.showRoomForm = false;
    this.editingRoomId = null;
  }

  saveRoom(): void {
    if (!this.roomHostelId) {
      this.errorMessage = 'Please select a hostel.';
      return;
    }

    if (!this.roomNumber.trim() || this.roomCapacity < 1) {
      this.errorMessage = 'Room number and valid capacity are required.';
      return;
    }

    this.loading = true;
    this.clearMessages();

    if (this.editingRoomId) {
      this.hostelService
        .updateRoom(this.editingRoomId, {
          roomNumber: this.roomNumber.trim(),
          capacity: this.roomCapacity
        })
        .subscribe({
          next: () => {
            this.successMessage = 'Room updated successfully.';
            this.showRoomForm = false;
            this.loadRooms(this.roomHostelId!);
            this.loading = false;
          },
          error: (error) => {
            console.error('Update room error:', error);
            this.errorMessage =
              error?.error?.message || 'Unable to update room.';
            this.loading = false;
          }
        });
    } else {
      this.hostelService
        .createRoom({
          hostelId: this.roomHostelId,
          roomNumber: this.roomNumber.trim(),
          capacity: this.roomCapacity
        })
        .subscribe({
          next: () => {
            this.successMessage = 'Room created successfully.';
            this.showRoomForm = false;
            this.loadRooms(this.roomHostelId!);
            this.loading = false;
          },
          error: (error) => {
            console.error('Create room error:', error);
            this.errorMessage =
              error?.error?.message || 'Unable to create room.';
            this.loading = false;
          }
        });
    }
  }

  getAvailableRooms(): number {
    return this.rooms.filter(room => room.isAvailable).length;
  }

  getTotalRooms(): number {
    return this.rooms.length;
  }

  getOccupiedRooms(): number {
    return this.rooms.filter(room => !room.isAvailable).length;
  }

  approveApplication(application: HostelApplication): void {
    this.updateStatus(application.id, 'Approved');
  }

  startReject(application: HostelApplication): void {
    this.rejectionApplicationId = application.id;
    this.rejectionReason = '';
    this.clearMessages();
  }

  cancelReject(): void {
    this.rejectionApplicationId = null;
    this.rejectionReason = '';
  }

  rejectApplication(application: HostelApplication): void {
    if (!this.rejectionReason.trim()) {
      this.errorMessage = 'Please enter a rejection reason.';
      return;
    }

    this.updateStatus(
      application.id,
      'Rejected',
      this.rejectionReason.trim()
    );
  }

  private updateStatus(
    id: number,
    status: 'Approved' | 'Rejected',
    rejectionReason = ''
  ): void {
    this.loading = true;
    this.clearMessages();

    this.hostelService
      .updateApplicationStatus(id, status, rejectionReason)
      .subscribe({
        next: () => {
          this.successMessage =
            status === 'Approved'
              ? 'Application approved successfully.'
              : 'Application rejected successfully.';

          this.rejectionApplicationId = null;
          this.rejectionReason = '';

          this.loadApplications();
          this.loading = false;
        },
        error: (error) => {
          console.error('Update application error:', error);
          this.errorMessage =
            error?.error?.message ||
            'Unable to update application status.';
          this.loading = false;
        }
      });
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }
}