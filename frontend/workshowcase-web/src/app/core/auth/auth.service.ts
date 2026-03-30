import { computed, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AuthResponse } from './models/auth-response.model';
import { CurrentUser, UserType } from './models/current-user.model';

const TOKEN_KEY = 'wsc_token';
const USER_KEY = 'wsc_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly _token = signal<string | null>(
    localStorage.getItem(TOKEN_KEY),
  );

  private readonly _currentUser = signal<CurrentUser | null>(
    this.loadUserFromStorage(),
  );

  readonly isAuthenticated = computed(() => this._token() !== null);
  readonly currentUser = this._currentUser.asReadonly();
  readonly currentUserRole = computed<UserType | null>(
    () => this._currentUser()?.userType ?? null,
  );

  constructor(private readonly router: Router) {}

  getToken(): string | null {
    return this._token();
  }

  /**
   * Called after a successful login or register API response.
   * Stores token + user summary from AuthResponse.
   */
  setSessionFromAuthResponse(response: AuthResponse): void {
    const user: CurrentUser = {
      id: response.id,
      email: response.email,
      fullName: response.fullName,
      userType: response.userType as UserType,
      phone: null,
      profileImageUrl: null,
      status: 'Active',
      createdAt: new Date().toISOString(),
    };
    this.persistSession(response.token, user);
  }

  /**
   * Called after GET /api/auth/me to hydrate full user details.
   */
  setCurrentUser(user: CurrentUser): void {
    localStorage.setItem(USER_KEY, JSON.stringify(user));
    this._currentUser.set(user);
  }

  clearSession(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this._token.set(null);
    this._currentUser.set(null);
  }

  logout(): void {
    this.clearSession();
    this.router.navigate(['/auth/login']);
  }

  hasRole(role: UserType): boolean {
    return this._currentUser()?.userType === role;
  }

  private persistSession(token: string, user: CurrentUser): void {
    localStorage.setItem(TOKEN_KEY, token);
    localStorage.setItem(USER_KEY, JSON.stringify(user));
    this._token.set(token);
    this._currentUser.set(user);
  }

  private loadUserFromStorage(): CurrentUser | null {
    try {
      const raw = localStorage.getItem(USER_KEY);
      return raw ? (JSON.parse(raw) as CurrentUser) : null;
    } catch {
      return null;
    }
  }
}
