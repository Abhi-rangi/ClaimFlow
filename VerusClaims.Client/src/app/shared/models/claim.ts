export interface Claim {
  id: number;
  claimNumber: string;
  claimantName: string;
  claimantEmail: string;
  claimantPhone: string;
  claimAmount: number;
  incidentDate: Date;
  filedDate: Date;
  status: string;
  description: string;
  notes?: string;
  createdAt: Date;
  updatedAt?: Date;
}

