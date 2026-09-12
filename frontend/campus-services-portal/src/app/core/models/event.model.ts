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
export interface CreateEvent {
  title: string;
  description: string;
  venue: string;
  eventDate: string;
  capacity: number;
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
  status: string;
  rejectReason: string | null;
}