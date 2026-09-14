export interface StudentProfile {
  id: number;
  fullName: string;
  indexNumber: string;
  email: string;
  faculty: string;
  contactNumber: string;
  role: string;
  isActive: boolean;
  createdAt: string;
}

export interface UpdateStudentProfile {
  fullName: string;
  email: string;
  faculty: string;
  contactNumber: string;
}