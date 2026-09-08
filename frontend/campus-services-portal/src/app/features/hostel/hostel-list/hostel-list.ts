import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  OnInit,
  inject
} from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { finalize } from 'rxjs';
import {
  Hostel,
  HostelApplication
} from '../../../core/models/hostel.model';
import { HostelService } from '../../../core/services/hostel';
import { Navbar } from '../../../shared/navbar/navbar';

type HostelTab = 'hostels' | 'apply' | 'applications';

@Component({
  selector: 'app-hostel-list',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    Navbar
  ],
  templateUrl: './hostel-list.html',
  styleUrl: './hostel-list.css'
})
export class HostelList implements OnInit {
  private readonly hostelService = inject(HostelService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly changeDetector = inject(ChangeDetectorRef);

  activeTab: HostelTab = 'hostels';

  hostels: Hostel[] = [];
  applications: HostelApplication[] = [];

  isLoadingHostels = true;
  isLoadingApplications = true;
  isSubmitting = false;

  errorMessage = '';
  successMessage = '';

  readonly applicationForm =
    this.formBuilder.nonNullable.group({
      hostelId: [
        0,
        [
          Validators.required,
          Validators.min(1)
        ]
      ],
      semester: [
        '',
        [
          Validators.required,
          Validators.maxLength(50)
        ]
      ],
      specialRequirements: [
        '',
        Validators.maxLength(500)
      ]
    });

  ngOnInit(): void {
    this.loadHostels();
    this.loadApplications();
  }

  setActiveTab(tab: HostelTab): void {
    this.activeTab = tab;
    this.errorMessage = '';
    this.successMessage = '';
  }

  selectHostel(hostel: Hostel): void {
    this.applicationForm.patchValue({
      hostelId: hostel.id
    });

    this.activeTab = 'apply';
    this.errorMessage = '';
    this.successMessage = '';
  }

  getSelectedHostelName(): string {
    const hostelId =
      this.applicationForm.controls.hostelId.value;

    return this.hostels.find(
      hostel => hostel.id === hostelId
    )?.name ?? 'Select a hostel';
  }

  loadHostels(): void {
    this.isLoadingHostels = true;
    this.errorMessage = '';

    this.hostelService.getHostels()
      .pipe(
        finalize(() => {
          this.isLoadingHostels = false;
          this.changeDetector.markForCheck();
        })
      )
      .subscribe({
        next: hostels => {
          this.hostels = hostels;
        },
        error: error => {
          this.errorMessage =
            error.error?.message ??
            'Unable to load hostels.';
        }
      });
  }

  loadApplications(): void {
    this.isLoadingApplications = true;

    this.hostelService.getMyApplications()
      .pipe(
        finalize(() => {
          this.isLoadingApplications = false;
          this.changeDetector.markForCheck();
        })
      )
      .subscribe({
        next: applications => {
          this.applications = applications;
        },
        error: error => {
          this.errorMessage =
            error.error?.message ??
            'Unable to load your hostel applications.';
        }
      });
  }

  submitApplication(): void {
    if (
      this.applicationForm.invalid ||
      this.isSubmitting
    ) {
      this.applicationForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    const formValue =
      this.applicationForm.getRawValue();

    this.hostelService.createApplication({
      hostelId: formValue.hostelId,
      semester: formValue.semester.trim(),
      specialRequirements:
        formValue.specialRequirements.trim()
    })
      .pipe(
        finalize(() => {
          this.isSubmitting = false;
          this.changeDetector.markForCheck();
        })
      )
      .subscribe({
        next: application => {
          this.applications = [
            application,
            ...this.applications
          ];

          this.applicationForm.reset({
            hostelId: 0,
            semester: '',
            specialRequirements: ''
          });

          this.successMessage =
            'Hostel application submitted successfully.';

          this.activeTab = 'applications';
        },
        error: error => {
          this.errorMessage =
            error.error?.message ??
            'Unable to submit the hostel application.';
        }
      });
  }

  getStatusClass(status: string): string {
    return status
      .trim()
      .toLowerCase()
      .replace(/\s+/g, '-');
  }
}