import { Component, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { AuthService } from '@auth0/auth0-angular';
import { Observable, of } from 'rxjs';
import { ClaimService } from '../../services/claim.service';
import { Claim } from '../../../../shared/models/claim';
import { ClaimFormComponent } from '../claim-form/claim-form.component';
import { ToastService } from '../../../../core/services/toast.service';

@Component({
  selector: 'app-claims-list',
  templateUrl: './claims-list.component.html',
  styleUrls: ['./claims-list.component.css']
})
export class ClaimsListComponent implements OnInit {
  displayedColumns: string[] = [
    'claimNumber',
    'claimantName',
    'claimantEmail',
    'claimAmount',
    'status',
    'filedDate',
    'actions'
  ];
  dataSource = new MatTableDataSource<Claim>();
  loading = false;
  statusFilter = '';
  
  // TODO: TBD - Role-based access control
  // Currently disabled - roles not available in user object from Auth0
  // Need to configure Auth0 Action to add roles to token
  // Once roles are available, re-enable role-based UI restrictions
  userRoles$: Observable<string[]>;
  canCreate$: Observable<boolean>;
  canEdit$: Observable<boolean>;
  canDelete$: Observable<boolean>;

  constructor(
    private claimService: ClaimService,
    private dialog: MatDialog,
    private toastService: ToastService,
    private authService: AuthService
  ) {
    // TODO: TBD - Extract roles from user claims once Auth0 Action is configured
    // For now, allow all users full access
    this.userRoles$ = of([]); // Empty roles for now
    
    // TODO: TBD - Enable role-based permissions once roles are available in token
    // Temporarily allow all operations for all authenticated users
    this.canCreate$ = of(true);
    this.canEdit$ = of(true);
    this.canDelete$ = of(true);
  }

  ngOnInit(): void {
    this.loadClaims();
  }

  loadClaims(): void {
    this.loading = true;
    if (this.statusFilter) {
      this.claimService.getClaimsByStatus(this.statusFilter).subscribe({
        next: (claims) => {
          this.dataSource.data = claims;
          this.loading = false;
        },
        error: () => {
          this.toastService.showError('Error loading claims');
          this.loading = false;
        }
      });
    } else {
      this.claimService.getClaims().subscribe({
        next: (claims) => {
          this.dataSource.data = claims;
          this.loading = false;
        },
        error: () => {
          this.toastService.showError('Error loading claims');
          this.loading = false;
        }
      });
    }
  }

  openAddDialog(): void {
    const dialogRef = this.dialog.open(ClaimFormComponent, {
      width: '600px',
      data: { claim: null }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadClaims();
      }
    });
  }

  openEditDialog(claim: Claim): void {
    const dialogRef = this.dialog.open(ClaimFormComponent, {
      width: '600px',
      data: { claim: { ...claim } }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadClaims();
      }
    });
  }

  deleteClaim(id: number): void {
    if (confirm('Are you sure you want to delete this claim?')) {
      this.claimService.deleteClaim(id).subscribe({
        next: () => {
          this.toastService.showSuccess('Claim deleted successfully');
          this.loadClaims();
        },
        error: () => {
          this.toastService.showError('Error deleting claim');
        }
      });
    }
  }

  onStatusFilterChange(): void {
    this.loadClaims();
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount);
  }

  formatDate(date: Date | string): string {
    if (!date) return '';
    const d = typeof date === 'string' ? new Date(date) : date;
    return d.toLocaleDateString('en-US');
  }
}

