/**
 * Matches LoginRequest from Features/Auth/DTOs/Requests/LoginRequest.cs
 * POST /api/auth/login
 */
export interface LoginRequest {
  email: string;
  password: string;
}
