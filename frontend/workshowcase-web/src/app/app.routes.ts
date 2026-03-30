import { Routes } from '@angular/router';
import { MainLayout } from './layout/main-layout/main-layout';
import { authGuard } from './core/auth/auth.guard';
import { roleGuard } from './core/auth/role.guard';

export const routes: Routes = [
  {
    path: '',
    component: MainLayout,
    children: [
      { path: '', redirectTo: 'auth/login', pathMatch: 'full' },

      {
        path: 'auth',
        loadChildren: () =>
          import('./features/auth/auth.routes').then((m) => m.AUTH_ROUTES),
      },

      {
        path: 'works',
        loadChildren: () =>
          import('./features/works/works.routes').then((m) => m.WORKS_ROUTES),
      },

      {
        path: 'professionals',
        loadChildren: () =>
          import('./features/professionals/professionals.routes').then(
            (m) => m.PROFESSIONALS_ROUTES,
          ),
      },

      {
        path: 'account',
        canActivate: [authGuard],
        loadChildren: () =>
          import('./features/account/account.routes').then(
            (m) => m.ACCOUNT_ROUTES,
          ),
      },

      {
        path: 'admin',
        canActivate: [authGuard, roleGuard('Admin')],
        loadChildren: () =>
          import('./features/admin/admin.routes').then((m) => m.ADMIN_ROUTES),
      },
    ],
  },

  { path: '**', redirectTo: 'works' },
];
