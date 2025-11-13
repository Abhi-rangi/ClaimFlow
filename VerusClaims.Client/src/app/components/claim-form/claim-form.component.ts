import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ClaimService } from '../../services/claim.service';
import { Claim } from '../../models/claim';

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
    private snackBar: MatSnackBar,
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
            this.snackBar.open('Claim updated successfully', 'Close', { duration: 3000 });
            this.dialogRef.close(true);
          },
          error: (error) => {
            console.error('Error updating claim:', error);
            this.snackBar.open('Error updating claim', 'Close', { duration: 3000 });
          }
        });
      } else {
        this.claimService.createClaim(claim).subscribe({
          next: () => {
            this.snackBar.open('Claim created successfully', 'Close', { duration: 3000 });
            this.dialogRef.close(true);
          },
          error: (error) => {
            console.error('Error creating claim:', error);
            this.snackBar.open('Error creating claim', 'Close', { duration: 3000 });
          }
        });
      }
    }
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}

