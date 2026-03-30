import { Routes } from '@angular/router';
import { LoginPage } from './pages/login/login';

export const AUTH_ROUTES: Routes = [
  {
    path: 'login',
    component: LoginPage,
    title: 'התחברות | Workshowcase',
  },
];
