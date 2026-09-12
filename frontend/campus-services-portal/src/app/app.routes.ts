import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/admin.guard';

import { Login } from './features/auth/login/login';
import { ForgotPassword } from './features/auth/forgot-password/forgot-password';
import { VerifyOtp } from './features/auth/verify-otp/verify-otp';
import { ResetPassword } from './features/auth/reset-password/reset-password';

import { CertificateList } from './features/certificates/certificate-list/certificate-list';
import { ComplaintList } from './features/complaints/complaint-list/complaint-list';
import { Dashboard } from './features/dashboard/dashboard/dashboard';
import { EventList } from './features/events/event-list/event-list';
import { FeeList } from './features/fees/fee-list/fee-list';
import { FeeManagement } from './features/fees/fee-management/fee-management';

import { HostelList } from './features/hostel/hostel-list/hostel-list';
import { HostelManagement } from './features/hostel/hostel-management/hostel-management';

import { LabList } from './features/labs/lab-list/lab-list';
import { LabManagement } from './features/labs/lab-management/lab-management';

import { NotificationList } from './features/notifications/notification-list/notification-list';

import { StudentList } from './features/students/student-list/student-list';
import { StudentManagement } from './features/admin/student-management/student-management';
import { CertificateManagement } from './features/admin/certificate-management/certificate-management';
export const routes: Routes = [
  {
    path: 'login',
    component: Login,
    title: 'Login | Campus Services Portal'
  },

  {
    path: 'dashboard',
    component: Dashboard,
    canActivate: [adminGuard, authGuard],
    title: 'Dashboard | Campus Services Portal'
  },

  {
    path: 'profile',
    component: StudentList,
    canActivate: [authGuard],
    title: 'My Profile | Campus Services Portal'
  },

  {
    path: 'hostels',
    component: HostelList,
    canActivate: [authGuard],
    title: 'Hostel Accommodation | Campus Services Portal'
  },

  {
    path: 'admin/hostels',
    component: HostelManagement,
    canActivate: [authGuard, adminGuard],
    title: 'Hostel Applications | Campus Services Portal'
  },

  {
    path: 'labs',
    component: LabList,
    canActivate: [authGuard],
    title: 'Lab Reservations | Campus Services Portal'
  },

  {
    path: 'admin/labs',
    component: LabManagement,
    canActivate: [authGuard, adminGuard],
    title: 'Lab Bookings | Campus Services Portal'
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
    canActivate: [authGuard, adminGuard],
    title: 'Fee Management | Campus Services Portal'
  },

  {
    path: 'admin/students',
    component: StudentManagement,
    canActivate: [authGuard, adminGuard],
    title: 'Student Management | Campus Services Portal'
  },
  {
    path: 'admin/certificates',
    component: CertificateManagement,
    canActivate: [authGuard, adminGuard],
    title: 'Certificate Management | Campus Services Portal'
  },

  {
    path: 'notifications',
    component: NotificationList,
    canActivate: [authGuard],
    title: 'Notifications | Campus Services Portal'
  },

  {
    path: 'forgot-password',
    component: ForgotPassword,
    title: 'Forgot Password | Campus Services Portal'
  },

  {
    path: 'verify-otp',
    component: VerifyOtp,
    title: 'Verify OTP | Campus Services Portal'
  },

  {
    path: 'reset-password',
    component: ResetPassword,
    title: 'Reset Password | Campus Services Portal'
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
