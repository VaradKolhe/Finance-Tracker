import { apiClient } from './apiClient';
import type { Report } from '../types/api';

export async function generateReport(period: 'weekly' | 'monthly') {
  const { data } = await apiClient.post<Report>('/reports/generate', { period, fromUtc: null, toUtc: null });
  return data;
}
