import { Component, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ClaimService } from '../../services/claim.service';
import { Claim } from '../../models/claim';
import { ClaimFormComponent } from '../claim-form/claim-form.component';

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

  constructor(
    private claimService: ClaimService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) { }

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
        error: (error) => {
          console.error('Error loading claims:', error);
          this.snackBar.open('Error loading claims', 'Close', { duration: 3000 });
          this.loading = false;
        }
      });
    } else {
      this.claimService.getClaims().subscribe({
        next: (claims) => {
          this.dataSource.data = claims;
          this.loading = false;
        },
        error: (error) => {
          console.error('Error loading claims:', error);
          this.snackBar.open('Error loading claims', 'Close', { duration: 3000 });
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
          this.snackBar.open('Claim deleted successfully', 'Close', { duration: 3000 });
          this.loadClaims();
        },
        error: (error) => {
          console.error('Error deleting claim:', error);
          this.snackBar.open('Error deleting claim', 'Close', { duration: 3000 });
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

