import { Routes } from '@angular/router';

import { CertificateList } from './features/certificates/certificate-list/certificate-list';
import { ComplaintList } from './features/complaints/complaint-list/complaint-list';
import { EventList } from './features/events/event-list/event-list';

export const routes: Routes = [
  {
    path: 'events',
    component: EventList
  },
  {
    path: 'complaints',
    component: ComplaintList
  },
  {
    path: 'certificates',
    component: CertificateList
  },
  {
    path: '',
    redirectTo: 'events',
    pathMatch: 'full'
  },
  {
    path: '**',
    redirectTo: 'events'
  }
];