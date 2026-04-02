import {
  Alert,
  Box,
  Button,
  CircularProgress,
  IconButton,
  LinearProgress,
  Stack,
  Typography,
} from '@mui/material';
import AddRoundedIcon from '@mui/icons-material/AddRounded';
import DeleteOutlineRoundedIcon from '@mui/icons-material/DeleteOutlineRounded';
import EditRoundedIcon from '@mui/icons-material/EditRounded';
import { useEffect, useState } from 'react';
import BudgetDialog from '../components/forms/BudgetDialog';
import SectionCard from '../components/common/SectionCard';
import { useAuth } from '../hooks/useAuth';
import { parseApiError } from '../services/apiClient';
import { getCategories } from '../services/categoryService';
import { createBudget, deleteBudget, getBudgets, updateBudget } from '../services/budgetService';
import type { BudgetProgress, Category, CreateBudgetPayload } from '../types/api';
import { formatCurrency } from '../utils/formatters';

export default function BudgetPlannerPage() {
  const { user } = useAuth();
  const [budgets, setBudgets] = useState<BudgetProgress[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [selectedBudget, setSelectedBudget] = useState<BudgetProgress | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    loadData();
  }, []);

  async function loadData() {
    try {
      const [budgetData, categoryData] = await Promise.all([getBudgets(), getCategories()]);
      setBudgets(budgetData);
      setCategories(categoryData);
    } catch (requestError) {
      setError(parseApiError(requestError));
    } finally {
      setIsLoading(false);
    }
  }

  async function handleBudgetSubmit(payload: CreateBudgetPayload) {
    if (selectedBudget) {
      await updateBudget(selectedBudget.budgetId, payload);
    } else {
      await createBudget(payload);
    }

    setDialogOpen(false);
    setSelectedBudget(null);
    await loadData();
  }

  async function handleDeleteBudget(budgetId: number) {
    await deleteBudget(budgetId);
    await loadData();
  }

  if (isLoading) {
    return (
      <Box minHeight="40vh" display="grid" sx={{ placeItems: 'center' }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Stack spacing={3}>
      {error ? <Alert severity="error">{error}</Alert> : null}
      <SectionCard
        title="Budget Planner"
        subtitle="Create monthly category budgets and watch their progress in real time."
        action={
          <Button variant="contained" startIcon={<AddRoundedIcon />} onClick={() => setDialogOpen(true)}>
            Add budget
          </Button>
        }
      >
        <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', xl: 'repeat(2, 1fr)' }, gap: 2 }}>
          {budgets.map((budget) => (
            <Box
              key={budget.budgetId}
              sx={{
                p: 2.5,
                borderRadius: 4,
                border: (theme) => `1px solid ${theme.palette.divider}`,
              }}
            >
              <Stack direction="row" justifyContent="space-between" spacing={2}>
                <div>
                  <Typography variant="h6">{budget.categoryName}</Typography>
                  <Typography variant="body2" color="text.secondary">
                    Limit {formatCurrency(budget.limitAmount, user?.currencyCode)} · Spent{' '}
                    {formatCurrency(budget.spentAmount, user?.currencyCode)}
                  </Typography>
                </div>
                <Stack direction="row" spacing={1}>
                  <IconButton onClick={() => { setSelectedBudget(budget); setDialogOpen(true); }}>
                    <EditRoundedIcon />
                  </IconButton>
                  <IconButton color="error" onClick={() => handleDeleteBudget(budget.budgetId)}>
                    <DeleteOutlineRoundedIcon />
                  </IconButton>
                </Stack>
              </Stack>
              <LinearProgress
                variant="determinate"
                value={Math.min(budget.progressPercentage, 100)}
                color={budget.isExceeded ? 'warning' : 'primary'}
                sx={{ mt: 2.5, height: 10, borderRadius: 999 }}
              />
              <Stack direction="row" justifyContent="space-between" sx={{ mt: 1.5 }}>
                <Typography variant="body2" color="text.secondary">
                  {budget.progressPercentage.toFixed(1)}% used
                </Typography>
                <Typography variant="body2" color={budget.isExceeded ? 'warning.main' : 'text.secondary'}>
                  Remaining {formatCurrency(budget.remainingAmount, user?.currencyCode)}
                </Typography>
              </Stack>
            </Box>
          ))}
        </Box>
      </SectionCard>

      <BudgetDialog
        open={dialogOpen}
        categories={categories}
        budget={selectedBudget}
        onClose={() => {
          setDialogOpen(false);
          setSelectedBudget(null);
        }}
        onSubmit={handleBudgetSubmit}
      />
    </Stack>
  );
}
