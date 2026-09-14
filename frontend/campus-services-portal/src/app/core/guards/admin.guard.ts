import { inject } from '@angular/core';
import {
  CanActivateFn,
  Router
} from '@angular/router';

import { Auth } from '../services/auth';

export const adminGuard: CanActivateFn = () => {
  const authService = inject(Auth);
  const router = inject(Router);

  const currentUser = authService.getCurrentUser();

  if (
    currentUser &&
    currentUser.role.toLowerCase() === 'admin'
  ) {
    return true;
  }

  return router.createUrlTree(['/dashboard']);
};