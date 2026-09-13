export interface ComplaintItem {
  id: number;
  studentId: number;
  complaintCategoryId: number;
  categoryName: string;
  description: string;
  status: string;
  resolutionNote: string | null;
  createdAt: string;
}

export interface ComplaintCategory {
  id: number;
  name: string;
  isActive: boolean;
}

export interface CreateComplaint {
  studentId: number;
  complaintCategoryId: number | null;
  customCategoryName?: string;
  description: string;
}