import { Alert, Box, Button, FormControlLabel, Stack, Switch, TextField } from '@mui/material';
import { useEffect, useState } from 'react';
import SectionCard from '../components/common/SectionCard';
import { useAuth } from '../hooks/useAuth';
import { parseApiError } from '../services/apiClient';
import { getProfile, updateProfile } from '../services/profileService';

export default function ProfilePage() {
  const { syncUser } = useAuth();
  const [form, setForm] = useState({
    firstName: '',
    lastName: '',
    monthlyIncome: 0,
    financialGoal: '',
    currencyCode: 'USD',
    prefersDarkMode: false,
  });
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);

  useEffect(() => {
    async function loadProfile() {
      try {
        const profile = await getProfile();
        setForm({
          firstName: profile.firstName,
          lastName: profile.lastName,
          monthlyIncome: profile.monthlyIncome,
          financialGoal: profile.financialGoal,
          currencyCode: profile.currencyCode,
          prefersDarkMode: profile.prefersDarkMode,
        });
      } catch (requestError) {
        setError(parseApiError(requestError));
      } finally {
        setIsLoading(false);
      }
    }

    loadProfile();
  }, []);

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError('');
    setSuccess('');
    setIsSaving(true);

    try {
      const updatedProfile = await updateProfile(form);
      syncUser(updatedProfile);
      setSuccess('Profile saved successfully.');
    } catch (requestError) {
      setError(parseApiError(requestError));
    } finally {
      setIsSaving(false);
    }
  }

  return (
    <SectionCard
      title="Profile & Preferences"
      subtitle="Update income, goals, currency, and appearance preferences used across the app."
    >
      {error ? <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert> : null}
      {success ? <Alert severity="success" sx={{ mb: 2 }}>{success}</Alert> : null}
      <Box component="form" onSubmit={handleSubmit}>
        <Stack spacing={2.5}>
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '1fr 1fr' }, gap: 2 }}>
            <TextField
              label="First name"
              value={form.firstName}
              onChange={(event) => setForm({ ...form, firstName: event.target.value })}
              disabled={isLoading}
            />
            <TextField
              label="Last name"
              value={form.lastName}
              onChange={(event) => setForm({ ...form, lastName: event.target.value })}
              disabled={isLoading}
            />
          </Box>
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '1fr 220px' }, gap: 2 }}>
            <TextField
              label="Financial goal"
              multiline
              minRows={4}
              value={form.financialGoal}
              onChange={(event) => setForm({ ...form, financialGoal: event.target.value })}
              disabled={isLoading}
            />
            <Stack spacing={2}>
              <TextField
                label="Monthly income"
                type="number"
                value={form.monthlyIncome}
                onChange={(event) => setForm({ ...form, monthlyIncome: Number(event.target.value) })}
                disabled={isLoading}
              />
              <TextField
                label="Currency code"
                value={form.currencyCode}
                onChange={(event) => setForm({ ...form, currencyCode: event.target.value.toUpperCase() })}
                disabled={isLoading}
              />
            </Stack>
          </Box>
          <FormControlLabel
            control={
              <Switch
                checked={form.prefersDarkMode}
                onChange={(event) => setForm({ ...form, prefersDarkMode: event.target.checked })}
                disabled={isLoading}
              />
            }
            label="Dark mode preference"
          />
          <Button type="submit" variant="contained" sx={{ alignSelf: 'flex-start' }} disabled={isSaving || isLoading}>
            {isSaving ? 'Saving...' : 'Save profile'}
          </Button>
        </Stack>
      </Box>
    </SectionCard>
  );
}
