import { environment } from '../../../environments/environment';

const apiUrl = (path: string): string => `${environment.apiBaseUrl}${path}`;

export const API_ENDPOINTS = {
  transactions: apiUrl('/api/transactions'),
} as const;
