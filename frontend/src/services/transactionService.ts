import { apiClient } from './apiClient';
import type { CreateTransactionPayload, Transaction } from '../types/api';

export async function getTransactions() {
  const { data } = await apiClient.get<Transaction[]>('/transactions');
  return data;
}

export async function createTransaction(payload: CreateTransactionPayload) {
  const { data } = await apiClient.post<Transaction>('/transactions', payload);
  return data;
}

export async function updateTransaction(transactionId: number, payload: CreateTransactionPayload) {
  const { data } = await apiClient.put<Transaction>(`/transactions/${transactionId}`, payload);
  return data;
}

export async function deleteTransaction(transactionId: number) {
  await apiClient.delete(`/transactions/${transactionId}`);
}
