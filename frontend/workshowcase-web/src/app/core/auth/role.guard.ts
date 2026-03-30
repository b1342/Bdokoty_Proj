import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';
import { UserType } from './models/current-user.model';

/**
 * Factory guard for role-based route protection.
 * Usage in routes: canActivate: [authGuard, roleGuard('Admin')]
 */
export function roleGuard(requiredRole: UserType): CanActivateFn {
  return () => {
    const auth = inject(AuthService);
    const router = inject(Router);

    if (auth.hasRole(requiredRole)) {
      return true;
    }

    return router.parseUrl('/');
  };
}
