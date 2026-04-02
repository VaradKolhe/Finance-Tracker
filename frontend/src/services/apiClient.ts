import axios from 'axios';
import type { ApiErrorResponse } from '../types/api';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5067/api';
const SESSION_KEY = 'ai-budget-analyzer-session';

export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

apiClient.interceptors.request.use((config) => {
  const session = localStorage.getItem(SESSION_KEY);
  if (!session) {
    return config;
  }

  const parsedSession = JSON.parse(session) as { token?: string };
  if (parsedSession.token) {
    config.headers.Authorization = `Bearer ${parsedSession.token}`;
  }

  return config;
});

export function parseApiError(error: unknown) {
  if (axios.isAxiosError<ApiErrorResponse>(error)) {
    return error.response?.data?.message ?? 'Request failed.';
  }

  return 'Something went wrong.';
}

export { SESSION_KEY };
