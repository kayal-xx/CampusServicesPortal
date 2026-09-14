export type CertificateStatus =
  | 'Pending'
  | 'Approved'
  | 'Rejected'
  | 'Ready for Collection';

export type CertificateType =
  | 'Bonafide Certificate'
  | 'Transcript'
  | 'Completion Letter';

export interface CertificateRequestItem {
  id: number;
  studentId: number;
  certificateType: string;
  reason: string;
  copies: number;
  status: string;
  rejectionReason: string;
  requestedAt: string;
}

export interface CertificateDocumentRequest {
  certificateType: CertificateType;
  reason: string;
  copies: number;
}

export interface CreateCertificateRequest {
  studentId: number;
  documents: CertificateDocumentRequest[];
}

export const CERTIFICATE_TYPES: CertificateType[] = [
  'Bonafide Certificate',
  'Transcript',
  'Completion Letter'
];