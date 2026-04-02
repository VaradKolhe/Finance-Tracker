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
  ToggleButton,
  ToggleButtonGroup,
  Typography,
} from '@mui/material';
import DownloadRoundedIcon from '@mui/icons-material/DownloadRounded';
import { useEffect, useState } from 'react';
import SectionCard from '../components/common/SectionCard';
import StatCard from '../components/common/StatCard';
import { useAuth } from '../hooks/useAuth';
import { parseApiError } from '../services/apiClient';
import { generateReport } from '../services/reportService';
import type { Report } from '../types/api';
import { formatCurrency, formatDate } from '../utils/formatters';
import { exportReportPdf } from '../utils/pdfExport';

export default function ReportsPage() {
  const { user } = useAuth();
  const [period, setPeriod] = useState<'weekly' | 'monthly'>('monthly');
  const [report, setReport] = useState<Report | null>(null);
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    loadReport('monthly');
  }, []);

  async function loadReport(nextPeriod: 'weekly' | 'monthly') {
    setIsLoading(true);
    setError('');

    try {
      const reportData = await generateReport(nextPeriod);
      setReport(reportData);
    } catch (requestError) {
      setError(parseApiError(requestError));
    } finally {
      setIsLoading(false);
    }
  }

  async function handlePeriodChange(_: React.MouseEvent<HTMLElement>, nextPeriod: 'weekly' | 'monthly' | null) {
    if (!nextPeriod) {
      return;
    }

    setPeriod(nextPeriod);
    await loadReport(nextPeriod);
  }

  if (isLoading && !report) {
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
        title="Reports"
        subtitle="Generate weekly or monthly summaries and export them to PDF."
        action={
          <Stack direction="row" spacing={1.5}>
            <ToggleButtonGroup value={period} exclusive onChange={handlePeriodChange} size="small">
              <ToggleButton value="weekly">Weekly</ToggleButton>
              <ToggleButton value="monthly">Monthly</ToggleButton>
            </ToggleButtonGroup>
            <Button
              variant="outlined"
              startIcon={<DownloadRoundedIcon />}
              onClick={() => report && exportReportPdf(report, user?.currencyCode)}
              disabled={!report}
            >
              Export PDF
            </Button>
          </Stack>
        }
      >
        {report ? (
          <Stack spacing={3}>
            <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: 'repeat(4, 1fr)' }, gap: 2 }}>
              {report.metrics.map((metric) => (
                <StatCard
                  key={metric.label}
                  label={metric.label}
                  value={formatCurrency(metric.value, user?.currencyCode)}
                  footnote={metric.description}
                />
              ))}
            </Box>

            <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', xl: '1fr 1fr' }, gap: 3 }}>
              <SectionCard
                title="Recommendations"
                subtitle={`Report range: ${formatDate(report.fromUtc)} to ${formatDate(report.toUtc)}`}
              >
                <Stack spacing={1.5}>
                  {report.recommendations.map((recommendation) => (
                    <Alert key={recommendation} severity="info">
                      {recommendation}
                    </Alert>
                  ))}
                </Stack>
              </SectionCard>

              <SectionCard title="Budget Status" subtitle="Budget performance for the reporting period.">
                <Stack spacing={1.5}>
                  {report.budgets.map((budget) => (
                    <Box
                      key={budget.budgetId}
                      sx={{
                        p: 2,
                        borderRadius: 4,
                        border: (theme) => `1px solid ${theme.palette.divider}`,
                      }}
                    >
                      <Typography fontWeight={600}>{budget.categoryName}</Typography>
                      <Typography variant="body2" color="text.secondary">
                        {formatCurrency(budget.spentAmount, user?.currencyCode)} spent of{' '}
                        {formatCurrency(budget.limitAmount, user?.currencyCode)}
                      </Typography>
                    </Box>
                  ))}
                </Stack>
              </SectionCard>
            </Box>

            <SectionCard title="Transactions Included" subtitle="Detailed line items used for this report.">
              <Table size="small">
                <TableHead>
                  <TableRow>
                    <TableCell>Date</TableCell>
                    <TableCell>Category</TableCell>
                    <TableCell>Notes</TableCell>
                    <TableCell>Type</TableCell>
                    <TableCell align="right">Amount</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {report.transactions.map((transaction) => (
                    <TableRow key={transaction.id}>
                      <TableCell>{formatDate(transaction.transactionDateUtc)}</TableCell>
                      <TableCell>{transaction.categoryName}</TableCell>
                      <TableCell>{transaction.notes || 'No notes'}</TableCell>
                      <TableCell>{transaction.type}</TableCell>
                      <TableCell align="right">{formatCurrency(transaction.amount, user?.currencyCode)}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </SectionCard>
          </Stack>
        ) : (
          <Alert severity="warning">No report has been generated yet.</Alert>
        )}
      </SectionCard>
    </Stack>
  );
}
