import { createTheme } from '@mui/material/styles';

export function createAppTheme(isDarkMode: boolean) {
  return createTheme({
    palette: {
      mode: isDarkMode ? 'dark' : 'light',
      primary: {
        main: '#0f766e',
      },
      secondary: {
        main: '#f97316',
      },
      background: isDarkMode
        ? {
            default: '#09131a',
            paper: '#0f1c26',
          }
        : {
            default: '#f7fafc',
            paper: '#ffffff',
          },
    },
    shape: {
      borderRadius: 18,
    },
    typography: {
      fontFamily: '"IBM Plex Sans", sans-serif',
      h3: {
        fontFamily: '"Space Grotesk", sans-serif',
        fontWeight: 700,
      },
      h4: {
        fontFamily: '"Space Grotesk", sans-serif',
        fontWeight: 700,
      },
      h5: {
        fontFamily: '"Space Grotesk", sans-serif',
        fontWeight: 700,
      },
      h6: {
        fontFamily: '"Space Grotesk", sans-serif',
        fontWeight: 700,
      },
    },
    components: {
      MuiCard: {
        styleOverrides: {
          root: {
            borderRadius: 20,
            boxShadow: isDarkMode
              ? '0 16px 48px rgba(0,0,0,0.28)'
              : '0 18px 40px rgba(15, 23, 42, 0.08)',
          },
        },
      },
      MuiButton: {
        styleOverrides: {
          root: {
            borderRadius: 999,
            textTransform: 'none',
            fontWeight: 600,
          },
        },
      },
    },
  });
}
