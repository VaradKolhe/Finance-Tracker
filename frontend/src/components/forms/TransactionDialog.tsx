import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControlLabel,
  MenuItem,
  Stack,
  Switch,
  TextField,
} from '@mui/material';
import { useEffect, useState } from 'react';
import type { Category, CreateTransactionPayload, RecurringFrequency, Transaction, TransactionType } from '../../types/api';

interface TransactionDialogProps {
  open: boolean;
  loading?: boolean;
  categories: Category[];
  transaction?: Transaction | null;
  onClose: () => void;
  onSubmit: (payload: CreateTransactionPayload) => Promise<void>;
}

const recurringOptions: RecurringFrequency[] = ['Weekly', 'BiWeekly', 'Monthly'];

export default function TransactionDialog({
  open,
  loading,
  categories,
  transaction,
  onClose,
  onSubmit,
}: TransactionDialogProps) {
  const [form, setForm] = useState<CreateTransactionPayload>({
    amount: 0,
    type: 'Expense',
    categoryId: 0,
    transactionDateUtc: new Date().toISOString(),
    notes: '',
    isRecurring: false,
    recurringFrequency: null,
    nextOccurrenceDateUtc: null,
  });

  useEffect(() => {
    if (transaction) {
      setForm({
        amount: transaction.amount,
        type: transaction.type,
        categoryId: transaction.categoryId,
        transactionDateUtc: transaction.transactionDateUtc,
        notes: transaction.notes,
        isRecurring: transaction.isRecurring,
        recurringFrequency: transaction.recurringFrequency,
        nextOccurrenceDateUtc: transaction.nextOccurrenceDateUtc,
      });
      return;
    }

    setForm({
      amount: 0,
      type: 'Expense',
      categoryId: categories[0]?.id ?? 0,
      transactionDateUtc: new Date().toISOString(),
      notes: '',
      isRecurring: false,
      recurringFrequency: null,
      nextOccurrenceDateUtc: null,
    });
  }, [transaction, categories, open]);

  async function handleSubmit() {
    await onSubmit(form);
  }

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>{transaction ? 'Edit transaction' : 'Add transaction'}</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 2 }}>
            <TextField
              label="Amount"
              type="number"
              fullWidth
              value={form.amount}
              onChange={(event) => setForm({ ...form, amount: Number(event.target.value) })}
            />
            <TextField
              select
              label="Type"
              fullWidth
              value={form.type}
              onChange={(event) => setForm({ ...form, type: event.target.value as TransactionType })}
            >
              <MenuItem value="Income">Income</MenuItem>
              <MenuItem value="Expense">Expense</MenuItem>
            </TextField>
          </Box>
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 2 }}>
            <TextField
              select
              label="Category"
              fullWidth
              value={form.categoryId}
              onChange={(event) => setForm({ ...form, categoryId: Number(event.target.value) })}
            >
              {categories.map((category) => (
                <MenuItem key={category.id} value={category.id}>
                  {category.name}
                </MenuItem>
              ))}
            </TextField>
            <TextField
              label="Date"
              type="date"
              fullWidth
              value={form.transactionDateUtc.slice(0, 10)}
              onChange={(event) =>
                setForm({
                  ...form,
                  transactionDateUtc: new Date(`${event.target.value}T09:00:00`).toISOString(),
                })
              }
              InputLabelProps={{ shrink: true }}
            />
          </Box>
          <TextField
            label="Notes"
            multiline
            minRows={3}
            value={form.notes}
            onChange={(event) => setForm({ ...form, notes: event.target.value })}
          />
          <FormControlLabel
            control={
              <Switch
                checked={form.isRecurring}
                onChange={(event) =>
                  setForm({
                    ...form,
                    isRecurring: event.target.checked,
                    recurringFrequency: event.target.checked ? form.recurringFrequency ?? 'Monthly' : null,
                  })
                }
              />
            }
            label="Recurring transaction"
          />
          {form.isRecurring ? (
            <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 2 }}>
              <TextField
                select
                label="Recurring frequency"
                fullWidth
                value={form.recurringFrequency ?? 'Monthly'}
                onChange={(event) =>
                  setForm({ ...form, recurringFrequency: event.target.value as RecurringFrequency })
                }
              >
                {recurringOptions.map((option) => (
                  <MenuItem key={option} value={option}>
                    {option}
                  </MenuItem>
                ))}
              </TextField>
              <TextField
                label="Next occurrence"
                type="date"
                fullWidth
                value={form.nextOccurrenceDateUtc?.slice(0, 10) ?? ''}
                onChange={(event) =>
                  setForm({
                    ...form,
                    nextOccurrenceDateUtc: event.target.value
                      ? new Date(`${event.target.value}T09:00:00`).toISOString()
                      : null,
                  })
                }
                InputLabelProps={{ shrink: true }}
              />
            </Box>
          ) : null}
        </Stack>
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 3 }}>
        <Button onClick={onClose}>Cancel</Button>
        <Button variant="contained" onClick={handleSubmit} disabled={loading}>
          {transaction ? 'Save changes' : 'Create transaction'}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
