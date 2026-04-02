import { apiClient } from './apiClient';
import type { UserProfile } from '../types/api';

interface UpdateProfilePayload {
  firstName: string;
  lastName: string;
  monthlyIncome: number;
  financialGoal: string;
  currencyCode: string;
  prefersDarkMode: boolean;
}

export async function getProfile() {
  const { data } = await apiClient.get<UserProfile>('/profile');
  return data;
}

export async function updateProfile(payload: UpdateProfilePayload) {
  const { data } = await apiClient.put<UserProfile>('/profile', payload);
  return data;
}
