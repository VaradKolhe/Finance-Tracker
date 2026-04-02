import { apiClient } from './apiClient';
import type { AuthResponse } from '../types/api';

interface RegisterPayload {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  monthlyIncome: number;
  financialGoal: string;
  currencyCode: string;
  prefersDarkMode: boolean;
}

interface LoginPayload {
  email: string;
  password: string;
}

export async function login(payload: LoginPayload) {
  const { data } = await apiClient.post<AuthResponse>('/auth/login', payload);
  return data;
}

export async function register(payload: RegisterPayload) {
  const { data } = await apiClient.post<AuthResponse>('/auth/register', payload);
  return data;
}

export async function logout() {
  await apiClient.post('/auth/logout');
}
