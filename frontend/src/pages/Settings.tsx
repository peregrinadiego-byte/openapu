import React, { useState } from 'react';
import { Upload, Download, AlertCircle } from 'lucide-react';
import { Card } from '../components';
import { ioAPI } from '../services/api';
import { useAppStore } from '../store/useAppStore';

export const Settings: React.FC = () => {
  const [loading, setLoading] = useState(false);
  const { showNotification } = useAppStore();

  const handleImportResources = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    try {
      setLoading(true);
      await ioAPI.importResourcesCSV(file);
      showNotification('Recursos importados exitosamente', 'success');
    } catch (error) {
      showNotification('Error al importar recursos', 'error');
    } finally {
      setLoading(false);
    }
  };

  const handleDownloadTemplate = async () => {
    try {
      const blob = await ioAPI.getResourcesTemplate();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = 'plantilla-recursos.csv';
      a.click();
      showNotification('Plantilla descargada', 'success');
    } catch (error) {
      showNotification('Error al descargar plantilla', 'error');
    }
  };

  const handleBackup = async () => {
    try {
      setLoading(true);
      const blob = await ioAPI.backup();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `openapu-backup-${new Date().toISOString().split('T')[0]}.db`;
      a.click();
      showNotification('Base de datos respaldada', 'success');
    } catch (error) {
      showNotification('Error al crear respaldo', 'error');
    } finally {
      setLoading(false);
    }
  };

  const handleRestore = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    if (!confirm('⚠️ Esto reemplazará toda la base de datos actual. ¿Estás seguro?')) {
      return;
    }

    try {
      setLoading(true);
      await ioAPI.restore(file);
      showNotification('Base de datos restaurada. Recarga la página.', 'success');
      setTimeout(() => window.location.reload(), 2000);
    } catch (error) {
      showNotification('Error al restaurar base de datos', 'error');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="space-y-6 max-w-2xl">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold text-gray-900 dark:text-white">Configuración</h1>
        <p className="text-gray-600 dark:text-gray-400 mt-2">
          Importar, exportar y gestionar respaldos de base de datos
        </p>
      </div>

      {/* Import Resources */}
      <Card className="p-6">
        <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
          <Upload size={24} className="text-primary-600" />
          Importar Recursos
        </h2>

        <div className="space-y-4">
          <p className="text-sm text-gray-600 dark:text-gray-400">
            Carga un archivo CSV con tus recursos. Descarga la plantilla primero para ver el formato requerido.
          </p>

          <div className="flex flex-col gap-3">
            <button
              onClick={handleDownloadTemplate}
              disabled={loading}
              className="w-full px-6 py-3 bg-gray-200 dark:bg-gray-800 hover:bg-gray-300 dark:hover:bg-gray-700 disabled:bg-gray-400 text-gray-900 dark:text-white rounded-lg font-medium transition-colors flex items-center justify-center gap-2"
            >
              <Download size={20} />
              Descargar Plantilla
            </button>

            <div className="relative">
              <input
                type="file"
                accept=".csv"
                onChange={handleImportResources}
                disabled={loading}
                className="absolute inset-0 opacity-0 cursor-pointer"
              />
              <button
                disabled={loading}
                className="w-full px-6 py-3 bg-primary-600 hover:bg-primary-700 disabled:bg-gray-400 text-white rounded-lg font-medium transition-colors flex items-center justify-center gap-2"
              >
                <Upload size={20} />
                Seleccionar Archivo CSV
              </button>
            </div>
          </div>
        </div>
      </Card>

      {/* Backup & Restore */}
      <Card className="p-6">
        <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
          <Download size={24} className="text-primary-600" />
          Respaldo y Restauración
        </h2>

        <div className="space-y-4">
          <p className="text-sm text-gray-600 dark:text-gray-400">
            Crea respaldos de tu base de datos completa o restaura desde una copia anterior.
          </p>

          <div className="flex flex-col gap-3">
            <button
              onClick={handleBackup}
              disabled={loading}
              className="w-full px-6 py-3 bg-green-600 hover:bg-green-700 disabled:bg-gray-400 text-white rounded-lg font-medium transition-colors flex items-center justify-center gap-2"
            >
              <Download size={20} />
              Crear Respaldo
            </button>

            <div className="relative">
              <input
                type="file"
                accept=".db"
                onChange={handleRestore}
                disabled={loading}
                className="absolute inset-0 opacity-0 cursor-pointer"
              />
              <button
                disabled={loading}
                className="w-full px-6 py-3 bg-orange-600 hover:bg-orange-700 disabled:bg-gray-400 text-white rounded-lg font-medium transition-colors flex items-center justify-center gap-2"
              >
                <Upload size={20} />
                Restaurar desde Archivo
              </button>
            </div>
          </div>

          {/* Warning */}
          <div className="p-3 bg-orange-50 dark:bg-orange-900 border border-orange-200 dark:border-orange-800 rounded-lg flex items-start gap-3">
            <AlertCircle size={20} className="text-orange-600 dark:text-orange-200 flex-shrink-0 mt-0.5" />
            <div className="text-sm text-orange-800 dark:text-orange-200">
              <p className="font-semibold mb-1">⚠️ Advertencia Importante</p>
              <p>
                Restaurar una base de datos reemplazará todos los datos actuales.
                Asegúrate de crear un respaldo antes de restaurar.
              </p>
            </div>
          </div>
        </div>
      </Card>

      {/* API Info */}
      <Card className="p-6 bg-gradient-to-r from-blue-50 to-indigo-50 dark:from-blue-900 dark:to-indigo-900 border-blue-200 dark:border-blue-800">
        <h2 className="text-xl font-semibold mb-4">ℹ️ Información de API</h2>
        <div className="space-y-2 text-sm">
          <div>
            <p className="text-gray-600 dark:text-gray-300">Base URL de API:</p>
            <p className="font-mono text-gray-900 dark:text-white break-all">
              {process.env.REACT_APP_API_URL || 'http://localhost:5080'}
            </p>
          </div>
          <div>
            <p className="text-gray-600 dark:text-gray-300">Endpoints disponibles:</p>
            <ul className="mt-2 space-y-1 text-gray-700 dark:text-gray-300">
              <li>• GET /resources - Listar recursos</li>
              <li>• GET /apus - Listar APUs</li>
              <li>• GET /concepts - Listar conceptos</li>
              <li>• GET /budgets - Listar presupuestos</li>
              <li>• GET /system/status - Estado del sistema</li>
            </ul>
          </div>
        </div>
      </Card>
    </div>
  );
};
