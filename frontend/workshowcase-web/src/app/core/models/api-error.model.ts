/**
 * Normalized shape of all error responses from the backend
 * matching ErrorHandlingMiddleware output:
 * { message: string, errors?: Record<string, string[]> }
 */
export interface ApiError {
  status: number;
  message: string;
  errors: Record<string, string[]> | null;
}
