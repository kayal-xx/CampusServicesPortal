import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core'; import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import {
    CertificateRequestItem,
    CertificateStatus,
    CertificateType
} from '../../../core/models/certificate.model';

import { CertificateService } from '../../../core/services/certificate.service';

@Component({
    selector: 'app-certificate-management',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './certificate-management.html',
    styleUrl: './certificate-management.css'
})
export class CertificateManagement implements OnInit {

    requests: CertificateRequestItem[] = [];

    search = '';
    statusFilter = 'All Statuses';
    typeFilter = 'All Types';

    loading = false;
    errorMessage = '';
    successMessage = '';

    selectedRequest: CertificateRequestItem | null = null;

    editStatus: CertificateStatus = 'Pending';
    rejectionReason = '';
    updating = false;

    readonly statuses: string[] = [
        'All Statuses',
        'Pending',
        'Approved',
        'Rejected',
        'Ready for Collection'
    ];

    readonly certificateTypes: string[] = [
        'All Types',
        'Bonafide Certificate',
        'Transcript',
        'Completion Letter'
    ];

    constructor(
        private certificateService: CertificateService,
        private cdr: ChangeDetectorRef
    ) { }
    ngOnInit(): void {
        this.loadRequests();
    }

    loadRequests(): void {
        this.loading = true;
        this.errorMessage = '';

        const status =
            this.statusFilter === 'All Statuses'
                ? undefined
                : this.statusFilter;

        this.certificateService.getRequests(status).subscribe({
            next: (data) => {
                console.log('Certificate requests:', data);

                this.requests = Array.isArray(data) ? data : [];
                this.loading = false;

                this.cdr.detectChanges();
            },
            error: (error) => {
                console.error('Certificate request error:', error);

                this.requests = [];
                this.loading = false;

                this.errorMessage =
                    error?.error?.message ||
                    'Unable to load certificate requests.';

                this.cdr.detectChanges();
            }
        });
    }
    get filteredRequests(): CertificateRequestItem[] {
        const searchText = this.search.trim().toLowerCase();

        return this.requests.filter((request) => {
            const matchesSearch =
                !searchText ||
                this.requestNumber(request.id).toLowerCase().includes(searchText) ||
                String(request.studentId).includes(searchText) ||
                (request.certificateType || '').toLowerCase().includes(searchText);

            const matchesType =
                this.typeFilter === 'All Types' ||
                request.certificateType === this.typeFilter;

            return matchesSearch && matchesType;
        });
    }

    openUpdate(request: CertificateRequestItem): void {
        this.selectedRequest = request;
        this.editStatus =
            request.status as CertificateStatus;
        this.rejectionReason =
            request.rejectionReason || '';

        this.clearMessages();
    }

    closeUpdate(): void {
        this.selectedRequest = null;
        this.rejectionReason = '';
        this.updating = false;
    }

    updateStatus(): void {
        if (!this.selectedRequest) {
            return;
        }

        this.clearMessages();

        if (
            this.editStatus === 'Rejected' &&
            !this.rejectionReason.trim()
        ) {
            this.errorMessage =
                'Rejection reason is required.';

            return;
        }

        this.updating = true;

        this.certificateService.updateRequestStatus(
            this.selectedRequest.id,
            this.editStatus,
            this.rejectionReason.trim()
        ).subscribe({
            next: () => {
                if (this.selectedRequest) {
                    this.selectedRequest.status =
                        this.editStatus;

                    this.selectedRequest.rejectionReason =
                        this.editStatus === 'Rejected'
                            ? this.rejectionReason.trim()
                            : '';
                }

                this.successMessage =
                    'Certificate request updated successfully.';

                this.closeUpdate();
                this.loadRequests();
            },
            error: (error) => {
                console.error(error);

                this.errorMessage =
                    error?.error?.message ||
                    'Unable to update certificate request.';

                this.updating = false;
            }
        });
    }

    onStatusFilterChange(): void {
        this.loadRequests();
    }

    requestNumber(id: number): string {
        return `CERT-${id.toString().padStart(4, '0')}`;
    }

    statusClass(status: string): string {
        return status
            .trim()
            .toLowerCase()
            .replace(/\s+/g, '-');
    }

    formatDate(date: string): string {
        return new Date(date).toLocaleDateString(
            'en-GB',
            {
                day: '2-digit',
                month: 'short',
                year: 'numeric'
            }
        );
    }

    clearMessages(): void {
        this.errorMessage = '';
        this.successMessage = '';
    }
}