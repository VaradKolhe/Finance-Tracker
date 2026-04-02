import { apiClient } from './apiClient';
import type { AiAnalysis, Dashboard } from '../types/api';

export async function getDashboard() {
  const { data } = await apiClient.get<Dashboard>('/dashboard');
  return data;
}

export async function getAiAnalysis() {
  const { data } = await apiClient.get<AiAnalysis>('/ai-analysis');
  return data;
}
