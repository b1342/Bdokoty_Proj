import { environment } from '../../../environments/environment';

/**
 * Resolves a relative MediaUrl returned by the backend
 * (e.g. /uploads/work-media/file.jpg) into a full absolute URL.
 *
 * Backend: UploadResponse.MediaUrl is a relative path served by UseStaticFiles().
 * Use this helper wherever <img> or <video> src attributes need a media URL.
 */
export function resolveMediaUrl(relativePath: string | null | undefined): string {
  if (!relativePath) return '';
  if (relativePath.startsWith('http://') || relativePath.startsWith('https://')) {
    return relativePath;
  }
  const base = environment.apiBaseUrl.replace(/\/$/, '');
  const path = relativePath.startsWith('/') ? relativePath : `/${relativePath}`;
  return `${base}${path}`;
}
