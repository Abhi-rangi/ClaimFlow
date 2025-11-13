import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ClaimService } from '../../services/claim.service';
import { Claim } from '../../../../shared/models/claim';
import { ToastService } from '../../../../core/services/toast.service';

@Component({
  selector: 'app-claim-form',
  templateUrl: './claim-form.component.html',
  styleUrls: ['./claim-form.component.css']
})
export class ClaimFormComponent implements OnInit {
  claimForm: FormGroup;
  isEditMode = false;
  statusOptions = ['Pending', 'Under Review', 'Approved', 'Denied', 'Closed'];

  constructor(
    private fb: FormBuilder,
    private claimService: ClaimService,
    private dialogRef: MatDialogRef<ClaimFormComponent>,
    private toastService: ToastService,
    @Inject(MAT_DIALOG_DATA) public data: { claim: Claim | null }
  ) {
    this.claimForm = this.fb.group({
      claimNumber: [''],
      claimantName: ['', Validators.required],
      claimantEmail: ['', [Validators.required, Validators.email]],
      claimantPhone: ['', Validators.required],
      claimAmount: ['', [Validators.required, Validators.min(0)]],
      incidentDate: ['', Validators.required],
      filedDate: ['', Validators.required],
      status: ['Pending', Validators.required],
      description: ['', Validators.required],
      notes: ['']
    });
  }

  ngOnInit(): void {
    if (this.data.claim && this.data.claim.id) {
      this.isEditMode = true;
      this.claimForm.patchValue({
        ...this.data.claim,
        incidentDate: new Date(this.data.claim.incidentDate),
        filedDate: new Date(this.data.claim.filedDate)
      });
      this.claimForm.get('claimNumber')?.disable();
    }
  }

  onSubmit(): void {
    if (this.claimForm.valid) {
      const formValue = this.claimForm.getRawValue();
      const claim: Claim = {
        ...formValue,
        incidentDate: formValue.incidentDate,
        filedDate: formValue.filedDate
      };

      if (this.isEditMode && this.data.claim?.id) {
        claim.id = this.data.claim.id;
        this.claimService.updateClaim(claim.id, claim).subscribe({
          next: () => {
            this.toastService.showSuccess('Claim updated successfully');
            this.dialogRef.close(true);
          },
          error: () => {
            this.toastService.showError('Error updating claim');
          }
        });
      } else {
        this.claimService.createClaim(claim).subscribe({
          next: () => {
            this.toastService.showSuccess('Claim created successfully');
            this.dialogRef.close(true);
          },
          error: () => {
            this.toastService.showError('Error creating claim');
          }
        });
      }
    }
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}

