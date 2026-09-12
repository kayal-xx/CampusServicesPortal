export interface Hostel {
  id: number;
  name: string;
  location: string;
  isActive: boolean;
}

export interface HostelRoom {
  id: number;
  hostelId: number;
  roomNumber: string;
  capacity: number;
  occupiedCount: number;
  isAvailable: boolean;
}
export interface CreateHostelApplication {
  hostelId: number;
  semester: string;
  specialRequirements: string;
}

export interface HostelApplication {
  id: number;
  studentId: number;
  studentName: string;
  hostelId: number;
  hostelName: string;
  roomId: number | null;
  roomNumber: string | null;
  semester: string;
  specialRequirements: string;
  status: string;
  rejectionReason: string;
  createdAt: string;
}
