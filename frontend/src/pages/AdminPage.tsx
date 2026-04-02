import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
} from '@mui/material';
import { useEffect, useState } from 'react';
import SectionCard from '../components/common/SectionCard';
import StatCard from '../components/common/StatCard';
import { useAuth } from '../hooks/useAuth';
import { parseApiError } from '../services/apiClient';
import { getAdminDashboard } from '../services/adminService';
import { createCategory, deleteCategory } from '../services/categoryService';
import type { AdminDashboard } from '../types/api';
import { formatCurrency, formatDate } from '../utils/formatters';

export default function AdminPage() {
  const { user } = useAuth();
  const [dashboard, setDashboard] = useState<AdminDashboard | null>(null);
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const [categoryForm, setCategoryForm] = useState({ name: '', icon: 'admin_panel_settings', colorHex: '#0f766e' });

  useEffect(() => {
    if (user?.role === 'Admin') {
      loadDashboard();
    }
  }, [user?.role]);

  async function loadDashboard() {
    try {
      const adminDashboard = await getAdminDashboard();
      setDashboard(adminDashboard);
    } catch (requestError) {
      setError(parseApiError(requestError));
    } finally {
      setIsLoading(false);
    }
  }

  async function handleCreateSystemCategory() {
    if (!categoryForm.name.trim()) {
      return;
    }

    await createCategory({ ...categoryForm, isSystemDefined: true });
    setCategoryForm({ name: '', icon: 'admin_panel_settings', colorHex: '#0f766e' });
    await loadDashboard();
  }

  async function handleDeleteCategory(categoryId: number) {
    await deleteCategory(categoryId);
    await loadDashboard();
  }

  if (user?.role !== 'Admin') {
    return <Alert severity="warning">This section is available only to administrators.</Alert>;
  }

  if (isLoading) {
    return (
      <Box minHeight="40vh" display="grid" sx={{ placeItems: 'center' }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error || !dashboard) {
    return <Alert severity="error">{error || 'Unable to load admin data.'}</Alert>;
  }

  return (
    <Stack spacing={3}>
      <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: 'repeat(5, 1fr)' }, gap: 2 }}>
        <StatCard label="Users" value={dashboard.analytics.totalUsers.toString()} />
        <StatCard label="Categories" value={dashboard.analytics.totalCategories.toString()} />
        <StatCard label="Transactions" value={dashboard.analytics.totalTransactions.toString()} />
        <StatCard label="Total income" value={formatCurrency(dashboard.analytics.totalIncome, 'USD')} />
        <StatCard label="Total expenses" value={formatCurrency(dashboard.analytics.totalExpenses, 'USD')} />
      </Box>

      <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', xl: '1.2fr 0.8fr' }, gap: 3 }}>
        <SectionCard title="Users" subtitle="Global view of all registered users and their activity.">
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Name</TableCell>
                <TableCell>Email</TableCell>
                <TableCell>Role</TableCell>
                <TableCell>Transactions</TableCell>
                <TableCell>Created</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {dashboard.users.map((adminUser) => (
                <TableRow key={adminUser.id}>
                  <TableCell>{adminUser.fullName}</TableCell>
                  <TableCell>{adminUser.email}</TableCell>
                  <TableCell>{adminUser.role}</TableCell>
                  <TableCell>{adminUser.transactionCount}</TableCell>
                  <TableCell>{formatDate(adminUser.createdAtUtc)}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </SectionCard>

        <SectionCard title="System Categories" subtitle="Manage platform-level categories visible to all users.">
          <Stack spacing={2}>
            <TextField
              label="Category name"
              value={categoryForm.name}
              onChange={(event) => setCategoryForm({ ...categoryForm, name: event.target.value })}
            />
            <TextField
              label="Icon"
              value={categoryForm.icon}
              onChange={(event) => setCategoryForm({ ...categoryForm, icon: event.target.value })}
            />
            <TextField
              label="Color"
              value={categoryForm.colorHex}
              onChange={(event) => setCategoryForm({ ...categoryForm, colorHex: event.target.value })}
            />
            <Button variant="contained" onClick={handleCreateSystemCategory}>
              Create system category
            </Button>
            <Stack spacing={1}>
              {dashboard.categories.map((category) => (
                <Box
                  key={category.id}
                  sx={{
                    p: 1.5,
                    borderRadius: 4,
                    border: (theme) => `1px solid ${theme.palette.divider}`,
                    display: 'flex',
                    justifyContent: 'space-between',
                    alignItems: 'center',
                  }}
                >
                  <Stack>
                    <strong>{category.name}</strong>
                    <small>{category.isSystemDefined ? 'System category' : 'User category'}</small>
                  </Stack>
                  {category.isSystemDefined ? (
                    <Button color="error" onClick={() => handleDeleteCategory(category.id)}>
                      Delete
                    </Button>
                  ) : null}
                </Box>
              ))}
            </Stack>
          </Stack>
        </SectionCard>
      </Box>
    </Stack>
  );
}
