import { create } from 'zustand';
import * as types from '../types';

interface AppState {
  // Theme
  darkMode: boolean;
  toggleDarkMode: () => void;

  // UI State
  showNotification: (message: string, type: 'success' | 'error' | 'info') => void;
  notification: { message: string; type: 'success' | 'error' | 'info' } | null;
  clearNotification: () => void;

  // Loading states
  isLoading: boolean;
  setLoading: (loading: boolean) => void;

  // System Status
  systemStatus: types.SystemStatus | null;
  setSystemStatus: (status: types.SystemStatus | null) => void;
}

export const useAppStore = create<AppState>((set) => ({
  // Theme
  darkMode: localStorage.getItem('darkMode') === 'true',
  toggleDarkMode: () => {
    set((state) => {
      const newDarkMode = !state.darkMode;
      localStorage.setItem('darkMode', String(newDarkMode));
      if (newDarkMode) {
        document.documentElement.classList.add('dark');
      } else {
        document.documentElement.classList.remove('dark');
      }
      return { darkMode: newDarkMode };
    });
  },

  // UI State
  notification: null,
  showNotification: (message, type) => {
    set({ notification: { message, type } });
    setTimeout(() => {
      set({ notification: null });
    }, 4000);
  },
  clearNotification: () => set({ notification: null }),

  // Loading states
  isLoading: false,
  setLoading: (loading) => set({ isLoading: loading }),

  // System Status
  systemStatus: null,
  setSystemStatus: (status) => set({ systemStatus: status }),
}));
