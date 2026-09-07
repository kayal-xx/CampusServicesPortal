import { CommonModule } from '@angular/common';
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Fee, FeePayment } from '../../../core/services/fee';
import { StudentService } from '../../../core/services/student';
import { StudentProfile } from '../../../core/models/student.model';

@Component({
  selector: 'app-fee-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './fee-list.html',
  styleUrl: './fee-list.css'
})
export class FeeList implements OnInit {

  student: StudentProfile | null = null;
  fees: FeePayment[] = [];

  outstandingBalance = 0;
  totalPaid = 0;
  TotalFees = 0;

  loading = true;
  errorMessage = '';

  constructor(
    private feeService: Fee,
    private studentService: StudentService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadStudent();
  }

  loadStudent(): void {
    this.studentService.getMyProfile().subscribe({
      next: (data: StudentProfile) => {
        this.student = data;

        this.loadFees(data.id);
      },
      error: () => {
        this.errorMessage = 'Can\'t load student details.';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadFees(studentId: number): void {
    this.feeService.getByStudentId(studentId).subscribe({
      next: (data: FeePayment[]) => {
        this.fees = data;

        this.TotalFees = this.fees
          .reduce((total, fee) => total + fee.amount, 0);

        this.outstandingBalance = this.fees
          .filter(fee => !fee.isPaid)
          .reduce((total, fee) => total + fee.amount, 0);

        this.totalPaid = this.fees
          .filter(fee => fee.isPaid)
          .reduce((total, fee) => total + fee.amount, 0);

        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Can\'t load fee details.';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  payNow(fee: FeePayment): void {
    this.feeService.updateStatus(fee.id, true).subscribe({
      next: () => {
        this.loadFees(fee.studentId);
      },
      error: () => {
        this.errorMessage = 'Can\'t update payment status.';
      }
    });
  }
}