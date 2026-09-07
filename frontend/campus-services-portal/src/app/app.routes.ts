import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';
import { Login } from './features/auth/login/login';
import { CertificateList } from './features/certificates/certificate-list/certificate-list';
import { ComplaintList } from './features/complaints/complaint-list/complaint-list';
import { Dashboard } from './features/dashboard/dashboard/dashboard';
import { EventList } from './features/events/event-list/event-list';
import { StudentList } from './features/students/student-list/student-list';

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
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];