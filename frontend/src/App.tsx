import React, { useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { useAppStore } from './store/useAppStore';
import { Layout } from './components';
import { Dashboard } from './pages/Dashboard';
import { Resources } from './pages/Resources';
import { Apus } from './pages/Apus';
import { Concepts } from './pages/Concepts';
import { Budgets } from './pages/Budgets';
import { Settings } from './pages/Settings';

function App() {
  const { darkMode } = useAppStore();

  useEffect(() => {
    if (darkMode) {
      document.documentElement.classList.add('dark');
    } else {
      document.documentElement.classList.remove('dark');
    }
  }, [darkMode]);

  return (
    <Router>
      <Layout>
        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route path="/resources" element={<Resources />} />
          <Route path="/apus" element={<Apus />} />
          <Route path="/concepts" element={<Concepts />} />
          <Route path="/budgets" element={<Budgets />} />
          <Route path="/settings" element={<Settings />} />
          <Route path="*" element={<Navigate to="/" />} />
        </Routes>
      </Layout>
    </Router>
  );
}

export default App;
