import { apiClient } from './apiClient';
import type { Category } from '../types/api';

interface CategoryPayload {
  name: string;
  icon: string;
  colorHex: string;
  isSystemDefined?: boolean;
}

export async function getCategories() {
  const { data } = await apiClient.get<Category[]>('/categories');
  return data;
}

export async function createCategory(payload: CategoryPayload) {
  const { data } = await apiClient.post<Category>('/categories', payload);
  return data;
}

export async function updateCategory(categoryId: number, payload: CategoryPayload) {
  const { data } = await apiClient.put<Category>(`/categories/${categoryId}`, payload);
  return data;
}

export async function deleteCategory(categoryId: number) {
  await apiClient.delete(`/categories/${categoryId}`);
}
