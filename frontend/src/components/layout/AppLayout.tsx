import {
  AppBar,
  Avatar,
  Box,
  Divider,
  Drawer,
  IconButton,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Stack,
  Switch,
  Toolbar,
  Tooltip,
  Typography,
} from '@mui/material';
import DashboardRoundedIcon from '@mui/icons-material/DashboardRounded';
import ReceiptLongRoundedIcon from '@mui/icons-material/ReceiptLongRounded';
import SavingsRoundedIcon from '@mui/icons-material/SavingsRounded';
import AssessmentRoundedIcon from '@mui/icons-material/AssessmentRounded';
import PersonRoundedIcon from '@mui/icons-material/PersonRounded';
import AdminPanelSettingsRoundedIcon from '@mui/icons-material/AdminPanelSettingsRounded';
import MenuRoundedIcon from '@mui/icons-material/MenuRounded';
import LogoutRoundedIcon from '@mui/icons-material/LogoutRounded';
import { Outlet, useLocation, useNavigate } from 'react-router-dom';
import { useState } from 'react';
import { useAuth } from '../../hooks/useAuth';
import { updateProfile } from '../../services/profileService';

const drawerWidth = 280;

export default function AppLayout() {
  const [mobileOpen, setMobileOpen] = useState(false);
  const [isThemeSaving, setIsThemeSaving] = useState(false);
  const { user, logout, syncUser } = useAuth();
  const location = useLocation();
  const navigate = useNavigate();

  const navItems = [
    { label: 'Dashboard', path: '/', icon: <DashboardRoundedIcon /> },
    { label: 'Transactions', path: '/transactions', icon: <ReceiptLongRoundedIcon /> },
    { label: 'Budget Planner', path: '/budgets', icon: <SavingsRoundedIcon /> },
    { label: 'Reports', path: '/reports', icon: <AssessmentRoundedIcon /> },
    { label: 'Profile', path: '/profile', icon: <PersonRoundedIcon /> },
  ];

  if (user?.role === 'Admin') {
    navItems.push({ label: 'Admin', path: '/admin', icon: <AdminPanelSettingsRoundedIcon /> });
  }

  const currentTitle = navItems.find((item) => item.path === location.pathname)?.label ?? 'Dashboard';

  async function handleLogout() {
    await logout();
    navigate('/login');
  }

  async function handleThemeToggle(nextChecked: boolean) {
    if (!user) {
      return;
    }

    syncUser({ ...user, prefersDarkMode: nextChecked });
    setIsThemeSaving(true);
    try {
      const updatedProfile = await updateProfile({
        firstName: user.firstName,
        lastName: user.lastName,
        monthlyIncome: user.monthlyIncome,
        financialGoal: user.financialGoal,
        currencyCode: user.currencyCode,
        prefersDarkMode: nextChecked,
      });

      syncUser(updatedProfile);
    } finally {
      setIsThemeSaving(false);
    }
  }

  const drawerContent = (
    <Box sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <Box sx={{ px: 3, pt: 4, pb: 2 }}>
        <Typography variant="overline" color="text.secondary">
          Smart Personal Finance
        </Typography>
        <Typography variant="h5" sx={{ mt: 1 }}>
          AI Budget Spending Analyzer
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
          Plan budgets, catch overspending early, and turn recent activity into practical AI suggestions.
        </Typography>
      </Box>
      <Divider />
      <List sx={{ px: 2, py: 2, flexGrow: 1 }}>
        {navItems.map((item) => {
          const active = location.pathname === item.path;

          return (
            <ListItemButton
              key={item.path}
              selected={active}
              onClick={() => {
                navigate(item.path);
                setMobileOpen(false);
              }}
              sx={{
                mb: 1,
                borderRadius: 3,
                py: 1.2,
              }}
            >
              <ListItemIcon sx={{ minWidth: 42 }}>{item.icon}</ListItemIcon>
              <ListItemText primary={item.label} />
            </ListItemButton>
          );
        })}
      </List>
      <Box sx={{ px: 3, pb: 3 }}>
        <Box
          sx={{
            p: 2.5,
            borderRadius: 4,
            background: (theme) =>
              theme.palette.mode === 'dark'
                ? 'linear-gradient(135deg, rgba(15,118,110,0.22), rgba(249,115,22,0.18))'
                : 'linear-gradient(135deg, rgba(15,118,110,0.12), rgba(249,115,22,0.16))',
          }}
        >
          <Typography variant="subtitle2">Signed in as</Typography>
          <Typography variant="h6" sx={{ mt: 0.5 }}>
            {user?.firstName} {user?.lastName}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {user?.email}
          </Typography>
        </Box>
      </Box>
    </Box>
  );

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh' }}>
      <AppBar
        position="fixed"
        color="transparent"
        elevation={0}
        sx={{
          width: { md: `calc(100% - ${drawerWidth}px)` },
          ml: { md: `${drawerWidth}px` },
          backdropFilter: 'blur(14px)',
          borderBottom: (theme) => `1px solid ${theme.palette.divider}`,
        }}
      >
        <Toolbar sx={{ gap: 2 }}>
          <IconButton color="inherit" onClick={() => setMobileOpen(true)} sx={{ display: { md: 'none' } }}>
            <MenuRoundedIcon />
          </IconButton>
          <Box sx={{ flexGrow: 1 }}>
            <Typography variant="h6">{currentTitle}</Typography>
            <Typography variant="body2" color="text.secondary">
              {new Date().toLocaleDateString('en-US', { weekday: 'long', month: 'long', day: 'numeric' })}
            </Typography>
          </Box>
          <Stack direction="row" spacing={1.5} alignItems="center">
            <Typography variant="body2" color="text.secondary">
              Dark mode
            </Typography>
            <Tooltip title={isThemeSaving ? 'Saving preference...' : 'Toggle appearance'}>
              <span>
                <Switch
                  checked={Boolean(user?.prefersDarkMode)}
                  onChange={(event) => handleThemeToggle(event.target.checked)}
                  disabled={!user || isThemeSaving}
                />
              </span>
            </Tooltip>
            <Avatar sx={{ bgcolor: 'primary.main' }}>{user?.firstName?.[0] ?? 'U'}</Avatar>
            <Tooltip title="Logout">
              <IconButton color="inherit" onClick={handleLogout}>
                <LogoutRoundedIcon />
              </IconButton>
            </Tooltip>
          </Stack>
        </Toolbar>
      </AppBar>

      <Box component="nav" sx={{ width: { md: drawerWidth }, flexShrink: { md: 0 } }}>
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onClose={() => setMobileOpen(false)}
          ModalProps={{ keepMounted: true }}
          sx={{
            display: { xs: 'block', md: 'none' },
            '& .MuiDrawer-paper': { width: drawerWidth },
          }}
        >
          {drawerContent}
        </Drawer>
        <Drawer
          variant="permanent"
          sx={{
            display: { xs: 'none', md: 'block' },
            '& .MuiDrawer-paper': {
              width: drawerWidth,
              boxSizing: 'border-box',
              borderRight: (theme) => `1px solid ${theme.palette.divider}`,
            },
          }}
          open
        >
          {drawerContent}
        </Drawer>
      </Box>

      <Box component="main" sx={{ flexGrow: 1, width: { md: `calc(100% - ${drawerWidth}px)` } }}>
        <Toolbar />
        <Box sx={{ p: { xs: 2, md: 3 } }}>
          <Outlet />
        </Box>
      </Box>
    </Box>
  );
}
