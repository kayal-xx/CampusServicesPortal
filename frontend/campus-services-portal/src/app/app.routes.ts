import { Routes } from '@angular/router';

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
    path: '',
    redirectTo: 'events',
    pathMatch: 'full'
  },
  {
    path: '**',
    redirectTo: 'events'
  }
];