import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';
import { FormsModule } from '@angular/forms';

import {
  EventItem,
  EventRegistration
} from '../../../core/models/event.model';
import { EventService } from '../../../core/services/event.service';
import { Navbar } from '../../../shared/navbar/navbar';
import { Auth } from '../../../core/services/auth';
type EventFilter = 'all' | 'upcoming' | 'past';

@Component({
  selector: 'app-event-list',
  imports: [
    CommonModule,
    FormsModule,
    Navbar
  ],
  templateUrl: './event-list.html',
  styleUrl: './event-list.css'
})
export class EventList implements OnInit {
  isAdmin = false;
  isCreateEventOpen = false;
  isCreatingEvent = false;

  newEvent = {
    title: '',
    description: '',
    venue: '',
    eventDate: '',
    capacity: 0
  };
  events: EventItem[] = [];
  registrations: EventRegistration[] = [];

  adminRegistrations: EventRegistration[] = [];
  selectedParticipantsEvent: EventItem | null = null;
  isRejectModalOpen = false;
  rejectRegistrationId: number | null = null;
  rejectReason = '';
  isProcessingRegistrationId: number | null = null;

  searchText = '';
  activeFilter: EventFilter = 'all';

  isLoading = false;
  registeringEventId: number | null = null;
  cancellingEventId: number | null = null;

  selectedEvent: EventItem | null = null;

  errorMessage = '';
  successMessage = '';

  // Replace this value with the authenticated student's ID later.
  studentId = 0;

  constructor(
    private eventService: EventService,
    private cdr: ChangeDetectorRef,
    private authService: Auth
  ) { }

  ngOnInit(): void {
    const currentUser = this.authService.getCurrentUser();

    if (!currentUser) {
      this.errorMessage = 'Please sign in again.';
      return;
    }

    this.isAdmin = currentUser.role.toLowerCase() === 'admin';

    this.studentId = currentUser.studentId;

    this.loadEvents();

    if (this.isAdmin) {
      this.loadAdminRegistrations();
    } else {
      this.loadStudentRegistrations();
    }
  }

  get filteredEvents(): EventItem[] {
    const search = this.searchText.trim().toLowerCase();
    const now = new Date().getTime();

    return this.events.filter((event) => {
      const eventTime = new Date(event.eventDate).getTime();

      const matchesPeriod =
        this.activeFilter === 'all' ||
        (this.activeFilter === 'upcoming' && eventTime >= now) ||
        (this.activeFilter === 'past' && eventTime < now);

      const matchesSearch =
        !search ||
        event.title.toLowerCase().includes(search) ||
        event.description.toLowerCase().includes(search) ||
        event.venue.toLowerCase().includes(search);

      return matchesPeriod && matchesSearch;
    });
  }

  loadEvents(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.eventService.getEvents().subscribe({
      next: (events) => {
        this.events = events;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Event loading error:', error);

        this.errorMessage =
          'Unable to load events. Please check the backend API connection.';

        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadStudentRegistrations(): void {
    this.eventService
      .getStudentRegistrations(this.studentId)
      .subscribe({
        next: (registrations) => {
          this.registrations = registrations;
          this.cdr.detectChanges();
        },
        error: (error) => {
          console.error(
            'Student registration loading error:',
            error
          );

          this.registrations = [];
          this.cdr.detectChanges();
        }
      });
  }
  loadAdminRegistrations(): void {
    this.eventService.getAdminRegistrations().subscribe({
      next: (registrations) => {
        this.adminRegistrations = registrations;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Admin registration loading error:', error);

        this.adminRegistrations = [];
        this.errorMessage =
          'Unable to load event registrations.';

        this.cdr.detectChanges();
      }
    });
  }
  openParticipants(event: EventItem): void {
    this.selectedParticipantsEvent = event;
  }

  closeParticipants(): void {
    this.selectedParticipantsEvent = null;
  }

  getEventRegistrations(eventId: number): EventRegistration[] {
    return this.adminRegistrations.filter(
      registration => registration.eventId === eventId
    );
  }

  approveRegistration(registrationId: number): void {
    this.isProcessingRegistrationId = registrationId;
    this.errorMessage = '';
    this.successMessage = '';

    this.eventService.approveRegistration(registrationId).subscribe({
      next: () => {
        const registration = this.adminRegistrations.find(
          item => item.id === registrationId
        );

        if (registration) {
          registration.status = 'Approved';
          registration.rejectReason = null;
        }

        this.successMessage = 'Registration approved successfully.';
        this.isProcessingRegistrationId = null;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Approval error:', error);

        this.errorMessage =
          error.error?.message ??
          'Unable to approve registration.';

        this.isProcessingRegistrationId = null;
        this.cdr.detectChanges();
      }
    });
  }

  openRejectModal(registrationId: number): void {
    this.rejectRegistrationId = registrationId;
    this.rejectReason = '';
    this.isRejectModalOpen = true;
  }

  closeRejectModal(): void {
    this.isRejectModalOpen = false;
    this.rejectRegistrationId = null;
    this.rejectReason = '';
  }

  rejectRegistration(): void {
    if (
      this.rejectRegistrationId === null ||
      !this.rejectReason.trim()
    ) {
      this.errorMessage = 'Reject reason is required.';
      return;
    }

    this.isProcessingRegistrationId =
      this.rejectRegistrationId;

    this.errorMessage = '';
    this.successMessage = '';

    this.eventService.rejectRegistration(
      this.rejectRegistrationId,
      this.rejectReason.trim()
    ).subscribe({
      next: () => {
        const registration = this.adminRegistrations.find(
          item => item.id === this.rejectRegistrationId
        );

        if (registration) {
          registration.status = 'Rejected';
          registration.rejectReason =
            this.rejectReason.trim();
        }

        this.successMessage =
          'Registration rejected successfully.';

        this.isProcessingRegistrationId = null;
        this.closeRejectModal();
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Rejection error:', error);

        this.errorMessage =
          error.error?.message ??
          'Unable to reject registration.';

        this.isProcessingRegistrationId = null;
        this.cdr.detectChanges();
      }
    });
  }

  setFilter(filter: EventFilter): void {
    this.activeFilter = filter;
  }

  openDetails(event: EventItem): void {
    this.selectedEvent = event;
  }

  closeDetails(): void {
    this.selectedEvent = null;
  }

  isRegistered(eventId: number): boolean {
    return this.registrations.some(
      (registration) => registration.eventId === eventId
    );
  }

  getRegistration(
    eventId: number
  ): EventRegistration | undefined {
    return this.registrations.find(
      (registration) => registration.eventId === eventId
    );
  }

  register(event: EventItem): void {
    if (event.isFull || this.isRegistered(event.id)) {
      return;
    }

    this.registeringEventId = event.id;
    this.errorMessage = '';
    this.successMessage = '';

    this.eventService.registerForEvent({
      studentId: this.studentId,
      eventId: event.id
    }).subscribe({
      next: (registration) => {
        this.registrations = [
          ...this.registrations,
          registration
        ];

        event.registeredCount += 1;
        event.availableSeats = Math.max(
          event.capacity - event.registeredCount,
          0
        );
        event.isFull = event.availableSeats === 0;

        this.successMessage =
          `Registration for "${event.title}" was successful.`;

        this.registeringEventId = null;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Event registration error:', error);

        this.errorMessage =
          error.error?.message ??
          'Unable to register for the event.';

        this.registeringEventId = null;
        this.cdr.detectChanges();
      }
    });
  }

  cancelRegistration(event: EventItem): void {
    const registration = this.getRegistration(event.id);

    if (!registration) {
      this.errorMessage = 'Registration record was not found.';
      return;
    }

    this.cancellingEventId = event.id;
    this.errorMessage = '';
    this.successMessage = '';

    this.eventService.cancelRegistration(
      registration.id,
      this.studentId
    ).subscribe({
      next: () => {
        this.registrations = this.registrations.filter(
          (item) => item.id !== registration.id
        );

        event.registeredCount = Math.max(
          event.registeredCount - 1,
          0
        );

        event.availableSeats = Math.max(
          event.capacity - event.registeredCount,
          0
        );

        event.isFull = false;

        this.successMessage =
          `Registration for "${event.title}" was cancelled.`;

        this.cancellingEventId = null;
        this.selectedEvent = null;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Cancellation error:', error);

        this.errorMessage =
          error.error?.message ??
          'Unable to cancel the registration.';

        this.cancellingEventId = null;
        this.cdr.detectChanges();
      }
    });
  }

  registrationPercentage(event: EventItem): number {
    if (event.capacity <= 0) {
      return 0;
    }

    return Math.min(
      (event.registeredCount / event.capacity) * 100,
      100
    );
  }

  imageClass(index: number): string {
    const imageClasses = [
      'academic-image',
      'career-image',
      'social-image',
      'workshop-image'
    ];

    return imageClasses[index % imageClasses.length];
  }
  openCreateEvent(): void {
    this.isCreateEventOpen = true;
    this.errorMessage = '';
    this.successMessage = '';
  }

  closeCreateEvent(): void {
    this.isCreateEventOpen = false;

    this.newEvent = {
      title: '',
      description: '',
      venue: '',
      eventDate: '',
      capacity: 0
    };
  }

  createEvent(): void {
    if (
      !this.newEvent.title.trim() ||
      !this.newEvent.description.trim() ||
      !this.newEvent.venue.trim() ||
      !this.newEvent.eventDate ||
      this.newEvent.capacity <= 0
    ) {
      this.errorMessage = 'Please fill all event details correctly.';
      return;
    }

    this.isCreatingEvent = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.eventService.createEvent({
      title: this.newEvent.title.trim(),
      description: this.newEvent.description.trim(),
      venue: this.newEvent.venue.trim(),
      eventDate: this.newEvent.eventDate,
      capacity: this.newEvent.capacity
    }).subscribe({
      next: () => {
        this.isCreatingEvent = false;
        this.closeCreateEvent();
        this.successMessage = 'Event created successfully.';
        this.loadEvents();
      },
      error: (error) => {
        this.isCreatingEvent = false;
        this.errorMessage =
          error.error?.message ?? 'Unable to create event.';
      }
    });
  }
}