import {
  Alert,
  Box,
  Button,
  Chip,
  CircularProgress,
  Divider,
  List,
  ListItem,
  ListItemText,
  Stack,
  Typography,
} from '@mui/material';
import { useEffect, useState } from 'react';
import {
  Area,
  AreaChart,
  Bar,
  BarChart,
  Cell,
  Pie,
  PieChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts';
import SectionCard from '../components/common/SectionCard';
import StatCard from '../components/common/StatCard';
import { useAuth } from '../hooks/useAuth';
import { parseApiError } from '../services/apiClient';
import { getAiAnalysis, getDashboard } from '../services/dashboardService';
import { markNotificationRead } from '../services/notificationService';
import type { AiAnalysis, Dashboard, NotificationItem } from '../types/api';
import { formatCurrency, formatDate } from '../utils/formatters';

export default function DashboardPage() {
  const { user } = useAuth();
  const [dashboard, setDashboard] = useState<Dashboard | null>(null);
  const [analysis, setAnalysis] = useState<AiAnalysis | null>(null);
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    async function loadDashboard() {
      try {
        const [dashboardData, aiData] = await Promise.all([getDashboard(), getAiAnalysis()]);
        setDashboard(dashboardData);
        setAnalysis(aiData);
      } catch (requestError) {
        setError(parseApiError(requestError));
      } finally {
        setIsLoading(false);
      }
    }

    loadDashboard();
  }, []);

  async function handleMarkAsRead(notification: NotificationItem) {
    await markNotificationRead(notification.id);
    setDashboard((currentDashboard) =>
      currentDashboard
        ? {
            ...currentDashboard,
            notifications: currentDashboard.notifications.map((item) =>
              item.id === notification.id ? { ...item, isRead: true } : item,
            ),
          }
        : currentDashboard,
    );
  }

  if (isLoading) {
    return (
      <Box minHeight="50vh" display="grid" sx={{ placeItems: 'center' }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error || !dashboard) {
    return <Alert severity="error">{error || 'Unable to load dashboard data.'}</Alert>;
  }

  return (
    <Stack spacing={3}>
      <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: 'repeat(4, 1fr)' }, gap: 2 }}>
        <StatCard
          label="Income"
          value={formatCurrency(dashboard.summary.totalIncome, user?.currencyCode)}
          footnote="Total income for the selected month"
        />
        <StatCard
          label="Expenses"
          value={formatCurrency(dashboard.summary.totalExpenses, user?.currencyCode)}
          tone="secondary"
          footnote="Tracked expense outflow this month"
        />
        <StatCard
          label="Net savings"
          value={formatCurrency(dashboard.summary.netSavings, user?.currencyCode)}
          tone={dashboard.summary.netSavings >= 0 ? 'success' : 'warning'}
          footnote="Income minus expenses"
        />
        <StatCard
          label="Savings rate"
          value={`${dashboard.summary.savingsRatePercentage.toFixed(1)}%`}
          footnote="Share of income kept after expenses"
        />
      </Box>

      <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', xl: '1.4fr 1fr' }, gap: 3 }}>
        <SectionCard title="Monthly Trend" subtitle="Income versus expenses across the last six months.">
          <Box sx={{ height: 300 }}>
            <ResponsiveContainer width="100%" height="100%">
              <AreaChart data={dashboard.monthlyTrend}>
                <XAxis dataKey="label" />
                <YAxis />
                <Tooltip />
                <Area type="monotone" dataKey="income" stroke="#0f766e" fill="#99f6e4" fillOpacity={0.5} />
                <Area type="monotone" dataKey="expenses" stroke="#f97316" fill="#fdba74" fillOpacity={0.4} />
              </AreaChart>
            </ResponsiveContainer>
          </Box>
        </SectionCard>

        <SectionCard title="AI Spending Analyzer" subtitle="Generated from your most recent 30 days of activity.">
          {analysis ? (
            <Stack spacing={2}>
              <Alert severity="info">{analysis.summary}</Alert>
              <div>
                <Typography variant="subtitle2">Insights</Typography>
                <List dense>
                  {analysis.insights.map((insight) => (
                    <ListItem key={insight} sx={{ px: 0 }}>
                      <ListItemText primary={insight} />
                    </ListItem>
                  ))}
                </List>
              </div>
              <div>
                <Typography variant="subtitle2">Recommendations</Typography>
                <List dense>
                  {analysis.recommendations.map((recommendation) => (
                    <ListItem key={recommendation} sx={{ px: 0 }}>
                      <ListItemText primary={recommendation} />
                    </ListItem>
                  ))}
                </List>
              </div>
            </Stack>
          ) : (
            <Alert severity="warning">AI analysis is not available yet.</Alert>
          )}
        </SectionCard>
      </Box>

      <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', xl: '1fr 1fr' }, gap: 3 }}>
        <SectionCard title="Category Breakdown" subtitle="Where your expense money is concentrated this month.">
          <Box sx={{ height: 300 }}>
            <ResponsiveContainer width="100%" height="100%">
              <PieChart>
                <Pie
                  data={dashboard.categoryBreakdown}
                  dataKey="amount"
                  nameKey="categoryName"
                  innerRadius={70}
                  outerRadius={110}
                  paddingAngle={3}
                >
                  {dashboard.categoryBreakdown.map((entry) => (
                    <Cell key={entry.categoryId} fill={entry.categoryColorHex} />
                  ))}
                </Pie>
                <Tooltip />
              </PieChart>
            </ResponsiveContainer>
          </Box>
          <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
            {dashboard.categoryBreakdown.map((category) => (
              <Chip
                key={category.categoryId}
                label={`${category.categoryName} ${category.percentage.toFixed(1)}%`}
                sx={{ borderColor: category.categoryColorHex }}
                variant="outlined"
              />
            ))}
          </Stack>
        </SectionCard>

        <SectionCard title="Budget Progress" subtitle="Monitor category budgets before they cross the line.">
          <Box sx={{ height: 300 }}>
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={dashboard.budgets}>
                <XAxis dataKey="categoryName" />
                <YAxis />
                <Tooltip />
                <Bar dataKey="spentAmount" fill="#0f766e" radius={[8, 8, 0, 0]} />
                <Bar dataKey="limitAmount" fill="#f97316" radius={[8, 8, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </Box>
        </SectionCard>
      </Box>

      <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', xl: '1.2fr 0.8fr' }, gap: 3 }}>
        <SectionCard title="Recent Transactions" subtitle="Most recent activity across income and expenses.">
          <Stack spacing={1.5}>
            {dashboard.recentTransactions.map((transaction) => (
              <Box
                key={transaction.id}
                sx={{
                  p: 2,
                  borderRadius: 4,
                  border: (theme) => `1px solid ${theme.palette.divider}`,
                  display: 'flex',
                  justifyContent: 'space-between',
                  alignItems: 'center',
                }}
              >
                <Box>
                  <Typography fontWeight={600}>{transaction.categoryName}</Typography>
                  <Typography variant="body2" color="text.secondary">
                    {formatDate(transaction.transactionDateUtc)} · {transaction.notes || 'No notes'}
                  </Typography>
                </Box>
                <Typography color={transaction.type === 'Income' ? 'success.main' : 'text.primary'} fontWeight={700}>
                  {transaction.type === 'Income' ? '+' : '-'}
                  {formatCurrency(transaction.amount, user?.currencyCode)}
                </Typography>
              </Box>
            ))}
          </Stack>
        </SectionCard>

        <SectionCard title="Notifications" subtitle="Budget alerts and unusual spending signals.">
          <Stack spacing={1.5}>
            {dashboard.notifications.length === 0 ? (
              <Alert severity="success">No active alerts right now.</Alert>
            ) : (
              dashboard.notifications.map((notification) => (
                <Box
                  key={notification.id}
                  sx={{
                    p: 2,
                    borderRadius: 4,
                    border: (theme) => `1px solid ${theme.palette.divider}`,
                    opacity: notification.isRead ? 0.72 : 1,
                  }}
                >
                  <Stack direction="row" justifyContent="space-between" spacing={2}>
                    <div>
                      <Typography fontWeight={600}>{notification.title}</Typography>
                      <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
                        {notification.message}
                      </Typography>
                    </div>
                    {!notification.isRead ? (
                      <Button size="small" onClick={() => handleMarkAsRead(notification)}>
                        Mark read
                      </Button>
                    ) : null}
                  </Stack>
                  <Divider sx={{ my: 1.5 }} />
                  <Typography variant="caption" color="text.secondary">
                    {formatDate(notification.createdAtUtc)}
                  </Typography>
                </Box>
              ))
            )}
          </Stack>
        </SectionCard>
      </Box>
    </Stack>
  );
}
