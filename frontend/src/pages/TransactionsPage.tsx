import {
  Alert,
  Box,
  Button,
  Chip,
  CircularProgress,
  IconButton,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
} from '@mui/material';
import AddRoundedIcon from '@mui/icons-material/AddRounded';
import DeleteOutlineRoundedIcon from '@mui/icons-material/DeleteOutlineRounded';
import EditRoundedIcon from '@mui/icons-material/EditRounded';
import { useEffect, useState } from 'react';
import SectionCard from '../components/common/SectionCard';
import TransactionDialog from '../components/forms/TransactionDialog';
import { useAuth } from '../hooks/useAuth';
import { parseApiError } from '../services/apiClient';
import { createCategory, getCategories } from '../services/categoryService';
import {
  createTransaction,
  deleteTransaction,
  getTransactions,
  updateTransaction,
} from '../services/transactionService';
import type { Category, CreateTransactionPayload, Transaction } from '../types/api';
import { formatCurrency, formatDate } from '../utils/formatters';

export default function TransactionsPage() {
  const { user } = useAuth();
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  const [dialogOpen, setDialogOpen] = useState(false);
  const [selectedTransaction, setSelectedTransaction] = useState<Transaction | null>(null);
  const [categoryForm, setCategoryForm] = useState({ name: '', icon: 'label', colorHex: '#0f766e' });

  useEffect(() => {
    loadData();
  }, []);

  async function loadData() {
    try {
      const [transactionData, categoryData] = await Promise.all([getTransactions(), getCategories()]);
      setTransactions(transactionData);
      setCategories(categoryData);
    } catch (requestError) {
      setError(parseApiError(requestError));
    } finally {
      setIsLoading(false);
    }
  }

  async function handleTransactionSubmit(payload: CreateTransactionPayload) {
    if (selectedTransaction) {
      await updateTransaction(selectedTransaction.id, payload);
    } else {
      await createTransaction(payload);
    }

    setDialogOpen(false);
    setSelectedTransaction(null);
    await loadData();
  }

  async function handleDeleteTransaction(transactionId: number) {
    await deleteTransaction(transactionId);
    await loadData();
  }

  async function handleCategoryCreate() {
    if (!categoryForm.name.trim()) {
      return;
    }

    await createCategory({ ...categoryForm, isSystemDefined: false });
    setCategoryForm({ name: '', icon: 'label', colorHex: '#0f766e' });
    setCategories(await getCategories());
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
      <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', xl: '1.4fr 0.7fr' }, gap: 3 }}>
        <SectionCard
          title="Transactions"
          subtitle="Track income, expenses, notes, dates, and recurring activity."
          action={
            <Button variant="contained" startIcon={<AddRoundedIcon />} onClick={() => setDialogOpen(true)}>
              Add transaction
            </Button>
          }
        >
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Date</TableCell>
                <TableCell>Category</TableCell>
                <TableCell>Notes</TableCell>
                <TableCell>Type</TableCell>
                <TableCell align="right">Amount</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {transactions.map((transaction) => (
                <TableRow key={transaction.id} hover>
                  <TableCell>{formatDate(transaction.transactionDateUtc)}</TableCell>
                  <TableCell>
                    <Chip
                      label={transaction.categoryName}
                      variant="outlined"
                      sx={{ borderColor: transaction.categoryColorHex }}
                    />
                  </TableCell>
                  <TableCell>{transaction.notes || 'No notes'}</TableCell>
                  <TableCell>{transaction.type}</TableCell>
                  <TableCell align="right">{formatCurrency(transaction.amount, user?.currencyCode)}</TableCell>
                  <TableCell align="right">
                    <IconButton onClick={() => { setSelectedTransaction(transaction); setDialogOpen(true); }}>
                      <EditRoundedIcon />
                    </IconButton>
                    <IconButton color="error" onClick={() => handleDeleteTransaction(transaction.id)}>
                      <DeleteOutlineRoundedIcon />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </SectionCard>

        <SectionCard title="Custom Categories" subtitle="Add your own spending categories alongside the seeded defaults.">
          <Stack spacing={2}>
            <TextField
              label="Category name"
              value={categoryForm.name}
              onChange={(event) => setCategoryForm({ ...categoryForm, name: event.target.value })}
            />
            <TextField
              label="Icon name"
              helperText="Any Material icon label, for example: restaurant or school"
              value={categoryForm.icon}
              onChange={(event) => setCategoryForm({ ...categoryForm, icon: event.target.value })}
            />
            <TextField
              label="Color"
              value={categoryForm.colorHex}
              onChange={(event) => setCategoryForm({ ...categoryForm, colorHex: event.target.value })}
            />
            <Button variant="contained" onClick={handleCategoryCreate}>
              Create category
            </Button>
            <Stack direction="row" spacing={1} useFlexGap flexWrap="wrap">
              {categories.map((category) => (
                <Chip
                  key={category.id}
                  label={category.name}
                  variant={category.isSystemDefined ? 'filled' : 'outlined'}
                  sx={{ backgroundColor: category.isSystemDefined ? `${category.colorHex}20` : undefined }}
                />
              ))}
            </Stack>
          </Stack>
        </SectionCard>
      </Box>

      <TransactionDialog
        open={dialogOpen}
        categories={categories}
        transaction={selectedTransaction}
        onClose={() => {
          setDialogOpen(false);
          setSelectedTransaction(null);
        }}
        onSubmit={handleTransactionSubmit}
      />
    </Stack>
  );
}
