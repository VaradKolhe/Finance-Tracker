import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  Link,
  Stack,
  TextField,
  Typography,
} from '@mui/material';
import { useState } from 'react';
import { Link as RouterLink, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { parseApiError } from '../services/apiClient';

export default function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const { login } = useAuth();
  const [email, setEmail] = useState('admin@budgetai.local');
  const [password, setPassword] = useState('Admin@12345');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState('');

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError('');
    setIsSubmitting(true);

    try {
      await login(email, password);
      const destination = location.state?.from?.pathname ?? '/';
      navigate(destination);
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
      <Box
        sx={{
          width: '100%',
          maxWidth: 1120,
          display: 'grid',
          gridTemplateColumns: { xs: '1fr', md: '1.1fr 0.9fr' },
          gap: 3,
        }}
      >
        <Card
          sx={{
            p: { xs: 1, md: 3 },
            background:
              'linear-gradient(150deg, rgba(15,118,110,0.95), rgba(8,47,73,0.94) 48%, rgba(249,115,22,0.84) 100%)',
            color: 'white',
          }}
        >
          <CardContent sx={{ p: { xs: 3, md: 5 } }}>
            <Typography variant="overline" sx={{ opacity: 0.82 }}>
              Personal Finance Intelligence
            </Typography>
            <Typography variant="h3" sx={{ mt: 1, maxWidth: 520 }}>
              Understand where your money goes before the month gets away from you.
            </Typography>
            <Typography variant="h6" sx={{ mt: 2.5, maxWidth: 560, opacity: 0.88, fontWeight: 400 }}>
              Track transactions, monitor category budgets, receive unusual spending alerts, and let AI surface saving ideas
              from your recent behavior.
            </Typography>
            <Stack spacing={2} sx={{ mt: 5 }}>
              {[
                'Live dashboard for income, expenses, and savings rate',
                'AI-generated habit analysis and money-saving suggestions',
                'Budget thresholds, alerts, reports, and admin analytics',
              ].map((feature) => (
                <Box
                  key={feature}
                  sx={{
                    px: 2.5,
                    py: 1.75,
                    borderRadius: 4,
                    backdropFilter: 'blur(8px)',
                    backgroundColor: 'rgba(255,255,255,0.12)',
                  }}
                >
                  <Typography>{feature}</Typography>
                </Box>
              ))}
            </Stack>
          </CardContent>
        </Card>

        <Card>
          <CardContent sx={{ p: { xs: 3, md: 5 } }}>
            <Typography variant="h4">Welcome back</Typography>
            <Typography color="text.secondary" sx={{ mt: 1, mb: 4 }}>
              Sign in to continue managing your budgets and insights.
            </Typography>
            <Box component="form" onSubmit={handleSubmit}>
              <Stack spacing={2.5}>
                {error ? <Alert severity="error">{error}</Alert> : null}
                <TextField label="Email address" value={email} onChange={(event) => setEmail(event.target.value)} />
                <TextField
                  label="Password"
                  type="password"
                  value={password}
                  onChange={(event) => setPassword(event.target.value)}
                />
                <Button type="submit" variant="contained" size="large" disabled={isSubmitting}>
                  {isSubmitting ? 'Signing in...' : 'Sign in'}
                </Button>
                <Typography variant="body2" color="text.secondary">
                  New here?{' '}
                  <Link component={RouterLink} to="/register" underline="hover">
                    Create an account
                  </Link>
                </Typography>
              </Stack>
            </Box>
          </CardContent>
        </Card>
      </Box>
    </Box>
  );
}
