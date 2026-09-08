export interface Hostel {
  id: number;
  name: string;
  location: string;
  isActive: boolean;
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
  createdAt: string;
}
