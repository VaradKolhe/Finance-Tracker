export type UserRole = 'User' | 'Admin';
export type TransactionType = 'Income' | 'Expense';
export type NotificationType = 'Info' | 'BudgetExceeded' | 'UnusualSpending';
export type RecurringFrequency = 'Weekly' | 'BiWeekly' | 'Monthly';

export interface AuthenticatedUser {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole;
  monthlyIncome: number;
  financialGoal: string;
  currencyCode: string;
  prefersDarkMode: boolean;
}

export interface AuthResponse {
  token: string;
  expiresAtUtc: string;
  user: AuthenticatedUser;
}

export interface UserProfile extends AuthenticatedUser {}

export interface Category {
  id: number;
  name: string;
  icon: string;
  colorHex: string;
  isSystemDefined: boolean;
  userId: number | null;
}

export interface Transaction {
  id: number;
  amount: number;
  type: TransactionType;
  categoryId: number;
  categoryName: string;
  categoryColorHex: string;
  categoryIcon: string;
  transactionDateUtc: string;
  notes: string;
  isRecurring: boolean;
  recurringFrequency: RecurringFrequency | null;
  nextOccurrenceDateUtc: string | null;
}

export interface CreateTransactionPayload {
  amount: number;
  type: TransactionType;
  categoryId: number;
  transactionDateUtc: string;
  notes: string;
  isRecurring: boolean;
  recurringFrequency: RecurringFrequency | null;
  nextOccurrenceDateUtc: string | null;
}

export interface BudgetProgress {
  budgetId: number;
  categoryId: number;
  categoryName: string;
  categoryColorHex: string;
  limitAmount: number;
  spentAmount: number;
  remainingAmount: number;
  progressPercentage: number;
  isExceeded: boolean;
  month: number;
  year: number;
}

export interface CreateBudgetPayload {
  categoryId: number;
  limitAmount: number;
  alertThresholdPercentage: number;
  month: number;
  year: number;
}

export interface NotificationItem {
  id: number;
  type: NotificationType;
  title: string;
  message: string;
  isRead: boolean;
  createdAtUtc: string;
}

export interface DashboardSummary {
  totalIncome: number;
  totalExpenses: number;
  netSavings: number;
  savingsRatePercentage: number;
}

export interface CategoryBreakdown {
  categoryId: number;
  categoryName: string;
  categoryColorHex: string;
  amount: number;
  percentage: number;
}

export interface MonthlyTrendPoint {
  label: string;
  income: number;
  expenses: number;
}

export interface Dashboard {
  summary: DashboardSummary;
  categoryBreakdown: CategoryBreakdown[];
  monthlyTrend: MonthlyTrendPoint[];
  recentTransactions: Transaction[];
  budgets: BudgetProgress[];
  notifications: NotificationItem[];
}

export interface AiAnalysis {
  summary: string;
  insights: string[];
  recommendations: string[];
}

export interface ReportMetric {
  label: string;
  value: number;
  description: string;
}

export interface Report {
  period: string;
  fromUtc: string;
  toUtc: string;
  metrics: ReportMetric[];
  budgets: BudgetProgress[];
  transactions: Transaction[];
  recommendations: string[];
}

export interface AdminUser {
  id: number;
  fullName: string;
  email: string;
  role: UserRole;
  monthlyIncome: number;
  currencyCode: string;
  transactionCount: number;
  createdAtUtc: string;
}

export interface SystemAnalytics {
  totalUsers: number;
  totalCategories: number;
  totalTransactions: number;
  totalIncome: number;
  totalExpenses: number;
  activeAlerts: number;
}

export interface AdminDashboard {
  analytics: SystemAnalytics;
  users: AdminUser[];
  categories: Category[];
}

export interface ApiValidationError {
  propertyName: string;
  errorMessage: string;
}

export interface ApiErrorResponse {
  message: string;
  errors?: ApiValidationError[];
}
