export interface EventItem {
  id: number;
  title: string;
  description: string;
  venue: string;
  eventDate: string;
  capacity: number;
  registeredCount: number;
  availableSeats: number;
  isFull: boolean;
}

export interface CreateEventRegistration {
  studentId: number;
  eventId: number;
}

export interface EventRegistration {
  id: number;
  studentId: number;
  eventId: number;
  eventTitle: string;
  venue: string;
  eventDate: string;
  registeredAt: string;
}