import { apiClient } from './apiClient';
import type { BudgetProgress, CreateBudgetPayload } from '../types/api';

export async function getBudgets() {
  const { data } = await apiClient.get<BudgetProgress[]>('/budgets');
  return data;
}

export async function createBudget(payload: CreateBudgetPayload) {
  const { data } = await apiClient.post<BudgetProgress>('/budgets', payload);
  return data;
}

export async function updateBudget(budgetId: number, payload: CreateBudgetPayload) {
  const { data } = await apiClient.put<BudgetProgress>(`/budgets/${budgetId}`, payload);
  return data;
}

export async function deleteBudget(budgetId: number) {
  await apiClient.delete(`/budgets/${budgetId}`);
}
