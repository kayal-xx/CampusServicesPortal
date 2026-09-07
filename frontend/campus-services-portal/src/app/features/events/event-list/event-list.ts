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

type EventFilter = 'all' | 'upcoming' | 'past';

@Component({
  selector: 'app-event-list',
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './event-list.html',
  styleUrl: './event-list.css'
})
export class EventList implements OnInit {
  events: EventItem[] = [];
  registrations: EventRegistration[] = [];

  searchText = '';
  activeFilter: EventFilter = 'all';

  isLoading = false;
  registeringEventId: number | null = null;
  cancellingEventId: number | null = null;

  selectedEvent: EventItem | null = null;

  errorMessage = '';
  successMessage = '';

  // Replace this value with the authenticated student's ID later.
  readonly studentId = 1;

  constructor(
    private eventService: EventService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadEvents();
    this.loadStudentRegistrations();
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
}