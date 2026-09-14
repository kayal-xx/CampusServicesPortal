import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ChangeDetectorRef } from '@angular/core';
import { Subscription, timer, switchMap } from 'rxjs';

import {
  DashboardService,
  DashboardSummary
} from '../../../core/services/dashboard';


@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit, OnDestroy {

  summary: DashboardSummary | null = null;

  loading = true;
  errorMessage = '';

  private refreshSubscription?: Subscription;

  constructor(
    private dashboardService: DashboardService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadDashboard();

    // Refresh dashboard data every 5 seconds
    this.refreshSubscription = timer(5000, 5000)
      .pipe(
        switchMap(() => this.dashboardService.getSummary())
      )
      .subscribe({
        next: (data: DashboardSummary) => {
          this.summary = data;
          this.loading = false;
          this.errorMessage = '';
          this.cdr.detectChanges();
        },
        error: (error) => {
          console.error('Dashboard API error:', error);
          this.errorMessage = 'Dashboard data load panna mudiyala.';
          this.loading = false;
          this.cdr.detectChanges();
        }
      });
  }

  loadDashboard(): void {
    this.dashboardService.getSummary().subscribe({
      next: (data: DashboardSummary) => {
        this.summary = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Dashboard API error:', error);
        this.errorMessage = 'Dashboard data load panna mudiyala.';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  ngOnDestroy(): void {
    this.refreshSubscription?.unsubscribe();
  }
}