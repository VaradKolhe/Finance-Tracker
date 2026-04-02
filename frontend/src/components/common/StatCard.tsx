import { Box, Card, CardContent, Chip, Stack, Typography } from '@mui/material';

interface StatCardProps {
  label: string;
  value: string;
  tone?: 'primary' | 'secondary' | 'success' | 'warning';
  footnote?: string;
}

export default function StatCard({ label, value, tone = 'primary', footnote }: StatCardProps) {
  return (
    <Card>
      <CardContent sx={{ p: 3 }}>
        <Stack spacing={1.5}>
          <Chip label={label} color={tone} sx={{ alignSelf: 'flex-start' }} />
          <Typography variant="h4">{value}</Typography>
          {footnote ? (
            <Box>
              <Typography variant="body2" color="text.secondary">
                {footnote}
              </Typography>
            </Box>
          ) : null}
        </Stack>
      </CardContent>
    </Card>
  );
}
