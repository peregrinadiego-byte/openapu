import React, { useEffect, useState } from 'react';
import {
  Package,
  Layers2,
  BookOpen,
  BarChart3,
  TrendingUp,
  Activity,
} from 'lucide-react';
import { Card, StatCard } from '../components';
import { systemAPI } from '../services/api';
import { useAppStore } from '../store/useAppStore';
import * as types from '../types';

export const Dashboard: React.FC = () => {
  const [status, setStatus] = useState<types.SystemStatus | null>(null);
  const [loading, setLoading] = useState(true);
  const { showNotification } = useAppStore();

  useEffect(() => {
    const fetchStatus = async () => {
      try {
        setLoading(true);
        const data = await systemAPI.getStatus();
        setStatus(data);
      } catch (error) {
        showNotification('Error al cargar el estado del sistema', 'error');
      } finally {
        setLoading(false);
      }
    };

    fetchStatus();
    const interval = setInterval(fetchStatus, 10000); // Actualizar cada 10 segundos

    return () => clearInterval(interval);
  }, [showNotification]);

  if (loading) {
    return (
      <div className="flex justify-center items-center h-96">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div>
        <h1 className="text-3xl font-bold text-gray-900 dark:text-white">Dashboard</h1>
        <p className="text-gray-600 dark:text-gray-400 mt-2">
          Bienvenido a OpenAPU - Sistema de Gestión de Presupuestos
        </p>
      </div>

      {/* Quick Stats */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <StatCard
          label="Recursos"
          value={status?.resources || 0}
          icon={<Package size={24} />}
          color="blue"
        />
        <StatCard
          label="APUs"
          value={status?.apus || 0}
          icon={<Layers2 size={24} />}
          color="green"
        />
        <StatCard
          label="Conceptos"
          value={status?.concepts || 0}
          icon={<BookOpen size={24} />}
          color="purple"
        />
        <StatCard
          label="Presupuestos"
          value={status?.budgets || 0}
          icon={<BarChart3 size={24} />}
          color="amber"
        />
      </div>

      {/* Main Content Grid */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* System Info */}
        <div className="lg:col-span-2">
          <Card className="p-6">
            <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
              <Activity size={24} className="text-primary-600" />
              Estado del Sistema
            </h2>
            <div className="space-y-3">
              <div className="flex justify-between items-center pb-3 border-b border-gray-200 dark:border-gray-800">
                <span className="text-gray-600 dark:text-gray-400">Nombre</span>
                <span className="font-medium">{status?.name}</span>
              </div>
              <div className="flex justify-between items-center pb-3 border-b border-gray-200 dark:border-gray-800">
                <span className="text-gray-600 dark:text-gray-400">Versión</span>
                <span className="font-medium">{status?.version}</span>
              </div>
              <div className="flex justify-between items-center pb-3 border-b border-gray-200 dark:border-gray-800">
                <span className="text-gray-600 dark:text-gray-400">Base de Datos</span>
                <span className="flex items-center gap-2">
                  <span className="w-3 h-3 rounded-full bg-green-500"></span>
                  <span className="font-medium">{status?.database}</span>
                </span>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-gray-600 dark:text-gray-400">Última Verificación</span>
                <span className="font-medium text-sm">
                  {new Date(status?.checkedAtUtc || '').toLocaleString()}
                </span>
              </div>
            </div>
          </Card>
        </div>

        {/* Quick Actions */}
        <Card className="p-6">
          <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
            <TrendingUp size={24} className="text-primary-600" />
            Acciones Rápidas
          </h2>
          <div className="space-y-2">
            <a
              href="/resources"
              className="block w-full px-4 py-2 bg-primary-50 dark:bg-primary-900 text-primary-700 dark:text-primary-200 rounded-lg hover:bg-primary-100 dark:hover:bg-primary-800 transition-colors font-medium text-center"
            >
              Gestionar Recursos
            </a>
            <a
              href="/apus"
              className="block w-full px-4 py-2 bg-primary-50 dark:bg-primary-900 text-primary-700 dark:text-primary-200 rounded-lg hover:bg-primary-100 dark:hover:bg-primary-800 transition-colors font-medium text-center"
            >
              Crear APU
            </a>
            <a
              href="/budgets"
              className="block w-full px-4 py-2 bg-primary-50 dark:bg-primary-900 text-primary-700 dark:text-primary-200 rounded-lg hover:bg-primary-100 dark:hover:bg-primary-800 transition-colors font-medium text-center"
            >
              Nuevo Presupuesto
            </a>
            <a
              href="/settings"
              className="block w-full px-4 py-2 bg-primary-50 dark:bg-primary-900 text-primary-700 dark:text-primary-200 rounded-lg hover:bg-primary-100 dark:hover:bg-primary-800 transition-colors font-medium text-center"
            >
              Configuración
            </a>
          </div>
        </Card>
      </div>

      {/* Footer Info */}
      <Card className="p-6 bg-gradient-to-r from-primary-50 to-blue-50 dark:from-primary-900 dark:to-blue-900 border-primary-200 dark:border-primary-800">
        <h3 className="font-semibold mb-2">💡 Consejo Útil</h3>
        <p className="text-sm text-gray-700 dark:text-gray-300">
          Comienza creando tus recursos básicos (materiales, mano de obra, etc.), luego agrúpalos en APUs,
          y finalmente construye tus presupuestos con conceptos que incluyan márgenes de ganancia e costos indirectos.
        </p>
      </Card>
    </div>
  );
};
