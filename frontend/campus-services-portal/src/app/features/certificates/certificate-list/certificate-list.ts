import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';
import { FormsModule } from '@angular/forms';

import {
  CERTIFICATE_TYPES,
  CertificateDocumentRequest,
  CertificateRequestItem,
  CertificateType
} from '../../../core/models/certificate.model';
import { CertificateService } from '../../../core/services/certificate.service';

type CertificateTab = 'list' | 'new';
type CertificateFilter =
  | 'all'
  | 'Pending'
  | 'Approved'
  | 'Rejected'
  | 'Ready for Collection';

interface CertificateDraft {
  certificateType: CertificateType | '';
  reason: string;
  copies: number;
}

@Component({
  selector: 'app-certificate-list',
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './certificate-list.html',
  styleUrl: './certificate-list.css'
})
export class CertificateList implements OnInit {
  requests: CertificateRequestItem[] = [];

  readonly certificateTypes = CERTIFICATE_TYPES;

  activeTab: CertificateTab = 'list';
  activeFilter: CertificateFilter = 'all';

  selectedRequest: CertificateRequestItem | null = null;

  documents: CertificateDraft[] = [
    this.createEmptyDocument()
  ];

  isLoading = false;
  isSubmitting = false;

  errorMessage = '';
  successMessage = '';

  // Replace this value with the authenticated student ID
  // after authentication is fully connected.
  readonly studentId = 1;

  constructor(
    private certificateService: CertificateService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadRequests();
  }

  get filteredRequests(): CertificateRequestItem[] {
    if (this.activeFilter === 'all') {
      return this.requests;
    }

    return this.requests.filter(
      (request) => request.status === this.activeFilter
    );
  }

  setTab(tab: CertificateTab): void {
    this.activeTab = tab;
    this.errorMessage = '';
    this.successMessage = '';

    if (tab === 'new' && this.documents.length === 0) {
      this.documents = [
        this.createEmptyDocument()
      ];
    }
  }

  setFilter(filter: CertificateFilter): void {
    this.activeFilter = filter;
  }

  loadRequests(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.certificateService
      .getStudentRequests(this.studentId)
      .subscribe({
        next: (requests) => {
          this.requests = requests.sort(
            (first, second) =>
              new Date(second.requestedAt).getTime() -
              new Date(first.requestedAt).getTime()
          );

          this.isLoading = false;
          this.changeDetectorRef.detectChanges();
        },
        error: () => {
          this.requests = [];
          this.errorMessage =
            'Unable to load certificate requests. Please check the backend API connection.';
          this.isLoading = false;
          this.changeDetectorRef.detectChanges();
        }
      });
  }

  addDocument(): void {
    if (this.documents.length >= this.certificateTypes.length) {
      this.errorMessage =
        'All available certificate types have already been added.';
      return;
    }

    this.errorMessage = '';
    this.documents = [
      ...this.documents,
      this.createEmptyDocument()
    ];
  }

  removeDocument(index: number): void {
    if (this.documents.length === 1) {
      this.documents = [
        this.createEmptyDocument()
      ];
      return;
    }

    this.documents = this.documents.filter(
      (_, documentIndex) => documentIndex !== index
    );
  }

  isTypeSelected(
    certificateType: CertificateType,
    currentIndex: number
  ): boolean {
    return this.documents.some(
      (document, index) =>
        index !== currentIndex &&
        document.certificateType === certificateType
    );
  }

  submitRequest(): void {
    this.errorMessage = '';
    this.successMessage = '';

    const hasMissingType = this.documents.some(
      (document) => !document.certificateType
    );

    if (hasMissingType) {
      this.errorMessage =
        'Please select a certificate type for every document.';
      return;
    }

    const hasInvalidReason = this.documents.some(
      (document) =>
        document.reason.trim().length < 5 ||
        document.reason.trim().length > 500
    );

    if (hasInvalidReason) {
      this.errorMessage =
        'Each reason must contain between 5 and 500 characters.';
      return;
    }

    const hasInvalidCopies = this.documents.some(
      (document) =>
        document.copies < 1 ||
        document.copies > 10
    );

    if (hasInvalidCopies) {
      this.errorMessage =
        'The number of copies must be between 1 and 10.';
      return;
    }

    const selectedTypes = this.documents.map(
      (document) => document.certificateType
    );

    if (new Set(selectedTypes).size !== selectedTypes.length) {
      this.errorMessage =
        'The same certificate type cannot be requested twice.';
      return;
    }

    const requestDocuments: CertificateDocumentRequest[] =
      this.documents.map((document) => ({
        certificateType:
          document.certificateType as CertificateType,
        reason: document.reason.trim(),
        copies: document.copies
      }));

    this.isSubmitting = true;

    this.certificateService.createRequest({
      studentId: this.studentId,
      documents: requestDocuments
    }).subscribe({
      next: (createdRequests) => {
        this.requests = [
          ...createdRequests,
          ...this.requests
        ];

        this.documents = [
          this.createEmptyDocument()
        ];

        this.activeTab = 'list';
        this.activeFilter = 'all';
        this.successMessage =
          'Certificate request submitted successfully.';
        this.isSubmitting = false;
        this.changeDetectorRef.detectChanges();
      },
      error: (error) => {
        this.errorMessage =
          error.error?.message ??
          'Unable to submit the certificate request.';
        this.isSubmitting = false;
        this.changeDetectorRef.detectChanges();
      }
    });
  }

  viewDetails(request: CertificateRequestItem): void {
    this.selectedRequest = request;
  }

  closeDetails(): void {
    this.selectedRequest = null;
  }

  requestNumber(requestId: number): string {
    return `CERT-${requestId.toString().padStart(4, '0')}`;
  }

  statusClass(status: string): string {
    return status
      .trim()
      .toLowerCase()
      .replace(/\s+/g, '-');
  }

  private createEmptyDocument(): CertificateDraft {
    return {
      certificateType: '',
      reason: '',
      copies: 1
    };
  }
}