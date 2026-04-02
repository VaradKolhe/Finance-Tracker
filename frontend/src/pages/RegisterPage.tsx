import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  FormControlLabel,
  Link,
  Stack,
  Switch,
  TextField,
  Typography,
} from '@mui/material';
import { useState } from 'react';
import { Link as RouterLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { parseApiError } from '../services/apiClient';

export default function RegisterPage() {
  const navigate = useNavigate();
  const { register } = useAuth();
  const [form, setForm] = useState({
    firstName: 'Ava',
    lastName: 'Patel',
    email: 'ava@example.com',
    password: 'Budget@123',
    monthlyIncome: 5000,
    financialGoal: 'Build an emergency fund and reduce unnecessary food spending.',
    currencyCode: 'USD',
    prefersDarkMode: false,
  });
  const [error, setError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError('');
    setIsSubmitting(true);

    try {
      await register(form);
      navigate('/');
    } catch (requestError) {
      setError(parseApiError(requestError));
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <Box
      sx={{
        minHeight: '100vh',
        display: 'grid',
        placeItems: 'center',
        px: 2,
        py: 4,
      }}
    >
      <Card sx={{ width: '100%', maxWidth: 820 }}>
        <CardContent sx={{ p: { xs: 3, md: 5 } }}>
          <Typography variant="h4">Create your account</Typography>
          <Typography color="text.secondary" sx={{ mt: 1, mb: 4 }}>
            Start tracking income, expenses, budgets, and AI-generated insights in one place.
          </Typography>
          <Box component="form" onSubmit={handleSubmit}>
            <Stack spacing={2.5}>
              {error ? <Alert severity="error">{error}</Alert> : null}
              <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 2 }}>
                <TextField
                  label="First name"
                  value={form.firstName}
                  onChange={(event) => setForm({ ...form, firstName: event.target.value })}
                />
                <TextField
                  label="Last name"
                  value={form.lastName}
                  onChange={(event) => setForm({ ...form, lastName: event.target.value })}
                />
              </Box>
              <TextField
                label="Email address"
                value={form.email}
                onChange={(event) => setForm({ ...form, email: event.target.value })}
              />
              <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 2 }}>
                <TextField
                  label="Password"
                  type="password"
                  value={form.password}
                  onChange={(event) => setForm({ ...form, password: event.target.value })}
                />
                <TextField
                  label="Monthly income"
                  type="number"
                  value={form.monthlyIncome}
                  onChange={(event) => setForm({ ...form, monthlyIncome: Number(event.target.value) })}
                />
              </Box>
              <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 160px' }, gap: 2 }}>
                <TextField
                  label="Financial goal"
                  multiline
                  minRows={3}
                  value={form.financialGoal}
                  onChange={(event) => setForm({ ...form, financialGoal: event.target.value })}
                />
                <TextField
                  label="Currency"
                  value={form.currencyCode}
                  onChange={(event) => setForm({ ...form, currencyCode: event.target.value.toUpperCase() })}
                />
              </Box>
              <FormControlLabel
                control={
                  <Switch
                    checked={form.prefersDarkMode}
                    onChange={(event) => setForm({ ...form, prefersDarkMode: event.target.checked })}
                  />
                }
                label="Start in dark mode"
              />
              <Button type="submit" variant="contained" size="large" disabled={isSubmitting}>
                {isSubmitting ? 'Creating account...' : 'Create account'}
              </Button>
              <Typography variant="body2" color="text.secondary">
                Already have an account?{' '}
                <Link component={RouterLink} to="/login" underline="hover">
                  Sign in
                </Link>
              </Typography>
            </Stack>
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
}
