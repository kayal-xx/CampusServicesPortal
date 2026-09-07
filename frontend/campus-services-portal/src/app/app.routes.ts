import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { Login } from './features/auth/login/login';
import { Dashboard } from './features/dashboard/dashboard/dashboard';
import { EventList } from './features/events/event-list/event-list';

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
    path: 'events',
    component: EventList,
    canActivate: [authGuard],
    title: 'Events | Campus Services Portal'
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