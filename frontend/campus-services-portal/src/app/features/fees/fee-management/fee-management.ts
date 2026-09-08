import { CommonModule } from '@angular/common';
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Fee, FeePayment } from '../../../core/services/fee';

@Component({
  selector: 'app-fee-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './fee-management.html',
  styleUrl: './fee-management.css'
})
export class FeeManagement implements OnInit {

  fees: FeePayment[] = [];

  totalRecords = 0;
  paidCount = 0;
  unpaidCount = 0;
  outstandingAmount = 0;

  loading = true;
  errorMessage = '';
  searchText = '';
  statusFilter: 'All' | 'Paid' | 'Unpaid' = 'All';

  constructor(private feeService: Fee, private cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.loadFees();
  }

  loadFees(): void {
    this.feeService.getAll().subscribe({
      next: (data) => {
        this.fees = data;

        this.totalRecords = this.fees.length;

        this.paidCount = this.fees
          .filter(fee => fee.isPaid).length;

        this.unpaidCount = this.fees
          .filter(fee => !fee.isPaid).length;

        this.outstandingAmount = this.fees
          .filter(fee => !fee.isPaid)
          .reduce((total, fee) => total + fee.amount, 0);

        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'can\'t load fee records.';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  markAsPaid(fee: FeePayment): void {
    this.feeService.updateStatus(fee.id, true).subscribe({
      next: () => {
        this.loadFees();
      },
      error: () => {
        this.errorMessage = 'can\'t update payment status.';
        this.cdr.detectChanges();
      }
    });
  }
  get filteredFees(): FeePayment[] {
  return this.fees.filter(fee => {

    const matchesSearch =
      fee.feeType.toLowerCase().includes(this.searchText.toLowerCase()) ||
      fee.studentId.toString().includes(this.searchText);

    const matchesStatus =
      this.statusFilter === 'All' ||
      (this.statusFilter === 'Paid' && fee.isPaid) ||
      (this.statusFilter === 'Unpaid' && !fee.isPaid);

    return matchesSearch && matchesStatus;
  });
}
}