/**
 * Matches CurrentUserResponse from Features/Auth/DTOs/Responses/CurrentUserResponse.cs
 */
export type UserType = 'Professional' | 'Client' | 'Admin';
export type UserStatus = 'Active' | 'Inactive' | 'Pending';

export interface CurrentUser {
  id: string;
  email: string;
  fullName: string;
  phone: string | null;
  userType: UserType;
  profileImageUrl: string | null;
  status: UserStatus;
  createdAt: string;
}
