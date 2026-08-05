import React, { useEffect } from 'react';
import { Link, useLocation } from 'react-router-dom';
import {
  Home,
  Package,
  Settings2,
  Layers2,
  BookOpen,
  BarChart3,
  Menu,
  X,
  Moon,
  Sun,
  Bell,
} from 'lucide-react';
import { useAppStore } from '../store/useAppStore';

interface LayoutProps {
  children: React.ReactNode;
}

export const Layout: React.FC<LayoutProps> = ({ children }) => {
  const location = useLocation();
  const { darkMode, toggleDarkMode, notification, clearNotification } = useAppStore();
  const [sidebarOpen, setSidebarOpen] = React.useState(true);

  useEffect(() => {
    if (darkMode) {
      document.documentElement.classList.add('dark');
    } else {
      document.documentElement.classList.remove('dark');
    }
  }, [darkMode]);

  const navItems = [
    { label: 'Dashboard', path: '/', icon: Home },
    { label: 'Recursos', path: '/resources', icon: Package },
    { label: 'APU', path: '/apus', icon: Layers2 },
    { label: 'Conceptos', path: '/concepts', icon: BookOpen },
    { label: 'Presupuestos', path: '/budgets', icon: BarChart3 },
    { label: 'Configuración', path: '/settings', icon: Settings2 },
  ];

  const isActive = (path: string) => {
    if (path === '/') {
      return location.pathname === '/';
    }
    return location.pathname.startsWith(path);
  };

  return (
    <div className="flex h-screen bg-white dark:bg-gray-950 text-gray-900 dark:text-white">
      {/* Sidebar */}
      <aside
        className={`${
          sidebarOpen ? 'w-64' : 'w-20'
        } bg-gradient-to-b from-primary-600 to-primary-800 text-white transition-all duration-300 flex flex-col`}
      >
        {/* Logo */}
        <div className="h-16 flex items-center justify-center border-b border-primary-700">
          <div className="flex items-center gap-2">
            <div className="w-8 h-8 bg-white rounded-lg flex items-center justify-center text-primary-700 font-bold text-lg">
              ✓
            </div>
            {sidebarOpen && <span className="font-bold text-lg">OpenAPU</span>}
          </div>
        </div>

        {/* Navigation */}
        <nav className="flex-1 overflow-y-auto px-2 py-4">
          <ul className="space-y-2">
            {navItems.map((item) => {
              const Icon = item.icon;
              const active = isActive(item.path);
              return (
                <li key={item.path}>
                  <Link
                    to={item.path}
                    className={`flex items-center gap-3 px-4 py-3 rounded-lg transition-colors ${
                      active
                        ? 'bg-white text-primary-700 font-semibold'
                        : 'text-primary-100 hover:bg-primary-700'
                    }`}
                    title={item.label}
                  >
                    <Icon size={20} />
                    {sidebarOpen && <span>{item.label}</span>}
                  </Link>
                </li>
              );
            })}
          </ul>
        </nav>

        {/* Footer */}
        <div className="border-t border-primary-700 p-2">
          <button
            onClick={toggleDarkMode}
            className="w-full flex items-center justify-center gap-2 px-4 py-3 rounded-lg text-primary-100 hover:bg-primary-700 transition-colors"
            title={darkMode ? 'Light mode' : 'Dark mode'}
          >
            {darkMode ? <Sun size={20} /> : <Moon size={20} />}
            {sidebarOpen && <span>{darkMode ? 'Claro' : 'Oscuro'}</span>}
          </button>
        </div>
      </aside>

      {/* Main Content */}
      <div className="flex-1 flex flex-col overflow-hidden">
        {/* Header */}
        <header className="h-16 bg-white dark:bg-gray-900 border-b border-gray-200 dark:border-gray-800 flex items-center justify-between px-6 shadow-sm">
          <button
            onClick={() => setSidebarOpen(!sidebarOpen)}
            className="p-2 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-lg transition-colors"
          >
            {sidebarOpen ? <X size={24} /> : <Menu size={24} />}
          </button>

          <div className="flex items-center gap-4">
            {notification && (
              <div
                className={`px-4 py-2 rounded-lg text-sm font-medium flex items-center gap-2 cursor-pointer ${
                  notification.type === 'success'
                    ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-100'
                    : notification.type === 'error'
                    ? 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-100'
                    : 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-100'
                }`}
                onClick={clearNotification}
              >
                <Bell size={16} />
                {notification.message}
              </div>
            )}
          </div>
        </header>

        {/* Content */}
        <main className="flex-1 overflow-auto">
          <div className="p-6">{children}</div>
        </main>
      </div>
    </div>
  );
};
