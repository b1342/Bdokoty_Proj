/**
 * Matches AuthResponse from Features/Auth/DTOs/Responses/AuthResponse.cs
 */
export interface AuthResponse {
  token: string;
  id: string;
  email: string;
  fullName: string;
  userType: string;
}
