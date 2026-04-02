import { apiClient } from './apiClient';
import type { AdminDashboard } from '../types/api';

export async function getAdminDashboard() {
  const { data } = await apiClient.get<AdminDashboard>('/admin/dashboard');
  return data;
}
