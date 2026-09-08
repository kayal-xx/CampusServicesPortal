import { CommonModule } from '@angular/common';
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Notification, NotificationItem } from '../../../core/services/notification';

@Component({
  imports: [CommonModule],
  selector: 'app-notification-list',
  styleUrl: './notification-list.css',
  templateUrl: './notification-list.html',
  standalone: true
})
export class NotificationList implements OnInit {
  notifications: NotificationItem[] = [];

  unreadCount = 0;
  loading = true;
  errorMessage = '';

  constructor(
    private notificationService: Notification,
    private changeDetectorRef: ChangeDetectorRef
  ) { }


  ngOnInit(): void {
    const studentId = 1; // Replace with the actual student ID
    this.loadNotifications(studentId);
  }

  loadNotifications(studentId: number): void {
    this.notificationService.getByStudentId(studentId).subscribe({
      next: (data) => {
        this.notifications = data;
        this.unreadCount = this.notifications
          .filter(item => !item.isRead).length;
        this.loading = false;
        this.changeDetectorRef.detectChanges();
      },
      error: (error) => {
        this.errorMessage = 'Failed to load notifications.';
        this.loading = false;
        this.changeDetectorRef.detectChanges();
      }
    });
  }

  markAsRead(item: NotificationItem): void {
    this.notificationService.updateReadStatus(item.id, true).subscribe({
      next: () => {
        this.loadNotifications(item.studentId);
      },
      error: () => {
        this.errorMessage = 'Failed to update notification status.';
      }
    });
  }
}