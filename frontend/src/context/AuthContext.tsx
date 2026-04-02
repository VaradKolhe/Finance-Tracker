import { createContext, useEffect, useState } from 'react';
import * as authService from '../services/authService';
import { SESSION_KEY } from '../services/apiClient';
import type { AuthenticatedUser } from '../types/api';

interface AuthContextValue {
  user: AuthenticatedUser | null;
  token: string | null;
  isAuthenticated: boolean;
  isReady: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (payload: {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    monthlyIncome: number;
    financialGoal: string;
    currencyCode: string;
    prefersDarkMode: boolean;
  }) => Promise<void>;
  logout: () => Promise<void>;
  syncUser: (user: AuthenticatedUser) => void;
}

export const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<AuthenticatedUser | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isReady, setIsReady] = useState(false);

  useEffect(() => {
    const session = localStorage.getItem(SESSION_KEY);
    if (session) {
      const parsed = JSON.parse(session) as { user: AuthenticatedUser; token: string };
      setUser(parsed.user);
      setToken(parsed.token);
    }

    setIsReady(true);
  }, []);

  async function login(email: string, password: string) {
    const response = await authService.login({ email, password });
    persistSession(response.token, response.user);
  }

  async function register(payload: {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    monthlyIncome: number;
    financialGoal: string;
    currencyCode: string;
    prefersDarkMode: boolean;
  }) {
    const response = await authService.register(payload);
    persistSession(response.token, response.user);
  }

  async function logout() {
    try {
      await authService.logout();
    } catch {
      // no-op: local session removal is the source of truth for logout UX
    }

    localStorage.removeItem(SESSION_KEY);
    setUser(null);
    setToken(null);
  }

  function syncUser(nextUser: AuthenticatedUser) {
    if (!token) {
      return;
    }

    persistSession(token, nextUser);
  }

  function persistSession(nextToken: string, nextUser: AuthenticatedUser) {
    localStorage.setItem(SESSION_KEY, JSON.stringify({ token: nextToken, user: nextUser }));
    setToken(nextToken);
    setUser(nextUser);
  }

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        isAuthenticated: Boolean(user && token),
        isReady,
        login,
        register,
        logout,
        syncUser,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}
