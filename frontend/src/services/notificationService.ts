import { apiClient } from './apiClient';
import type { NotificationItem } from '../types/api';

export async function getNotifications() {
  const { data } = await apiClient.get<NotificationItem[]>('/notifications');
  return data;
}

export async function markNotificationRead(notificationId: number) {
  await apiClient.put(`/notifications/${notificationId}/read`);
}
