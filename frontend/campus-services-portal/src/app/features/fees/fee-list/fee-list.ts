import { CommonModule } from '@angular/common';
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Fee, FeePayment } from '../../../core/services/fee';
import { Student, StudentDto } from '../../../core/services/student';

@Component({
  selector: 'app-fee-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './fee-list.html',
  styleUrl: './fee-list.css'
})
export class FeeList implements OnInit {

  student: StudentDto | null = null;
  fees: FeePayment[] = [];

  outstandingBalance = 0;
  totalPaid = 0;
  TotalFees = 0;

  loading = true;
  errorMessage = '';

  constructor(
    private feeService: Fee,
    private studentService: Student,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
   
    const studentId = 1;

    this.loadStudent(studentId);
    this.loadFees(studentId);
  }

  loadStudent(studentId: number): void {
    this.studentService.getById(studentId).subscribe({
      next: (data) => {
        this.student = data;
      },
      error: () => {
        this.errorMessage = 'can\'t load student details.';
      }
    });
  }

  loadFees(studentId: number): void {
  this.feeService.getByStudentId(studentId).subscribe({
    next: (data) => {
      console.log('Fee data received:', data);

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

      console.log('Loading:', this.loading);
    },
    error: (error) => {
      console.error('Fee API error:', error);

      this.errorMessage = 'can\'t load fee details.';
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
        this.errorMessage = 'can\'t update payment status.';
      }
    });
  }
}