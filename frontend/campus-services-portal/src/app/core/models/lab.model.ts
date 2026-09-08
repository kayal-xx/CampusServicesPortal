export interface Lab {
  id: number;
  name: string;
  location: string;
  capacity: number;
  isActive: boolean;
}

export interface LabBooking {
  id: number;
  studentId: number;
  studentName: string;
  labId: number;
  labName: string;
  bookingDate: string;
  startTime: string;
  endTime: string;
  status: string;
}

export interface CreateLabBooking {
  labId: number;
  bookingDate: string;
  startTime: string;
  endTime: string;
}

export interface ApiMessage {
  message: string;
}