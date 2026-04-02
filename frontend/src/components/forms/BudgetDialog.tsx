import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  MenuItem,
  Stack,
  TextField,
} from '@mui/material';
import { useEffect, useState } from 'react';
import type { BudgetProgress, Category, CreateBudgetPayload } from '../../types/api';

interface BudgetDialogProps {
  open: boolean;
  categories: Category[];
  budget?: BudgetProgress | null;
  loading?: boolean;
  onClose: () => void;
  onSubmit: (payload: CreateBudgetPayload) => Promise<void>;
}

export default function BudgetDialog({ open, categories, budget, loading, onClose, onSubmit }: BudgetDialogProps) {
  const [form, setForm] = useState<CreateBudgetPayload>({
    categoryId: 0,
    limitAmount: 0,
    alertThresholdPercentage: 90,
    month: new Date().getMonth() + 1,
    year: new Date().getFullYear(),
  });

  useEffect(() => {
    if (budget) {
      setForm({
        categoryId: budget.categoryId,
        limitAmount: budget.limitAmount,
        alertThresholdPercentage: 90,
        month: budget.month,
        year: budget.year,
      });
      return;
    }

    setForm({
      categoryId: categories[0]?.id ?? 0,
      limitAmount: 0,
      alertThresholdPercentage: 90,
      month: new Date().getMonth() + 1,
      year: new Date().getFullYear(),
    });
  }, [budget, categories, open]);

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>{budget ? 'Edit budget' : 'Create budget'}</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          <TextField
            select
            label="Category"
            value={form.categoryId}
            onChange={(event) => setForm({ ...form, categoryId: Number(event.target.value) })}
          >
            {categories.map((category) => (
              <MenuItem key={category.id} value={category.id}>
                {category.name}
              </MenuItem>
            ))}
          </TextField>
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 2 }}>
            <TextField
              label="Limit amount"
              type="number"
              fullWidth
              value={form.limitAmount}
              onChange={(event) => setForm({ ...form, limitAmount: Number(event.target.value) })}
            />
            <TextField
              label="Alert threshold %"
              type="number"
              fullWidth
              value={form.alertThresholdPercentage}
              onChange={(event) => setForm({ ...form, alertThresholdPercentage: Number(event.target.value) })}
            />
            <TextField
              label="Month"
              type="number"
              fullWidth
              value={form.month}
              onChange={(event) => setForm({ ...form, month: Number(event.target.value) })}
            />
            <TextField
              label="Year"
              type="number"
              fullWidth
              value={form.year}
              onChange={(event) => setForm({ ...form, year: Number(event.target.value) })}
            />
          </Box>
        </Stack>
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 3 }}>
        <Button onClick={onClose}>Cancel</Button>
        <Button variant="contained" disabled={loading} onClick={() => onSubmit(form)}>
          {budget ? 'Save budget' : 'Create budget'}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
