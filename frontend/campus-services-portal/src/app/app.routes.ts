import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';

import { Login } from './features/auth/login/login';
import { Dashboard } from './features/dashboard/dashboard/dashboard';
import { StudentList } from './features/students/student-list/student-list';
import { EventList } from './features/events/event-list/event-list';
import { ComplaintList } from './features/complaints/complaint-list/complaint-list';
import { CertificateList } from './features/certificates/certificate-list/certificate-list';

import { FeeList } from './features/fees/fee-list/fee-list';
import { FeeManagement } from './features/fees/fee-management/fee-management';
import { NotificationList } from './features/notifications/notification-list/notification-list';

export const routes: Routes = [
  {
    path: 'login',
    component: Login,
    title: 'Login | Campus Services Portal'
  },
  {
    path: 'dashboard',
    component: Dashboard,
    canActivate: [authGuard],
    title: 'Dashboard | Campus Services Portal'
  },
  {
    path: 'profile',
    component: StudentList,
    canActivate: [authGuard],
    title: 'My Profile | Campus Services Portal'
  },
  {
    path: 'events',
    component: EventList,
    canActivate: [authGuard],
    title: 'Events | Campus Services Portal'
  },
  {
    path: 'complaints',
    component: ComplaintList,
    canActivate: [authGuard],
    title: 'Complaints | Campus Services Portal'
  },
  {
    path: 'certificates',
    component: CertificateList,
    canActivate: [authGuard],
    title: 'Certificates | Campus Services Portal'
  },
  {
    path: 'fees',
    component: FeeList,
    canActivate: [authGuard],
    title: 'Fees & Payments | Campus Services Portal'
  },
  {
    path: 'admin/fees',
    component: FeeManagement,
    canActivate: [authGuard],
    title: 'Fee Management | Campus Services Portal'
  },
  {
    path: 'notifications',
    component: NotificationList,
    canActivate: [authGuard],
    title: 'Notifications | Campus Services Portal'
  },
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];