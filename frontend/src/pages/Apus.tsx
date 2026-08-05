import React, { useEffect, useState } from 'react';
import { Plus, Edit2, Trash2, Eye } from 'lucide-react';
import { Card, Table, Modal, Form } from '../components';
import * as types from '../types';
import { apuAPI, resourceAPI } from '../services/api';
import { useAppStore } from '../store/useAppStore';

interface ApuWithResources extends types.Apu {
  componentsWithResources?: Array<types.ApuComponent & { resource: types.Resource }>;
}

export const Apus: React.FC = () => {
  const [apus, setApus] = useState<ApuWithResources[]>([]);
  const [resources, setResources] = useState<types.Resource[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [selectedApu, setSelectedApu] = useState<ApuWithResources | null>(null);
  const { showNotification } = useAppStore();

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [apusData, resourcesData] = await Promise.all([
        apuAPI.getAll(),
        resourceAPI.getAll(),
      ]);
      setApus(apusData);
      setResources(resourcesData);
    } catch (error) {
      showNotification('Error al cargar APUs', 'error');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (values: Record<string, any>) => {
    try {
      await apuAPI.create(values as types.CreateApuCommand);
      showNotification('APU creado exitosamente', 'success');
      setShowModal(false);
      fetchData();
    } catch (error) {
      showNotification('Error al crear APU', 'error');
    }
  };

  const handleAddComponent = async (resourceId: string, quantity: number) => {
    try {
      if (selectedApu) {
        const updated = await apuAPI.addComponent(selectedApu.id, resourceId, quantity);
        showNotification('Componente agregado', 'success');
        setApus(apus.map(a => a.id === selectedApu.id ? updated : a));
        setSelectedApu(updated);
      }
    } catch (error) {
      showNotification('Error al agregar componente', 'error');
    }
  };

  const handleRemoveComponent = async (componentId: string) => {
    try {
      if (selectedApu) {
        const updated = await apuAPI.removeComponent(selectedApu.id, componentId);
        showNotification('Componente removido', 'success');
        setApus(apus.map(a => a.id === selectedApu.id ? updated : a));
        setSelectedApu(updated);
      }
    } catch (error) {
      showNotification('Error al remover componente', 'error');
    }
  };

  const formFields = [
    { name: 'key', label: 'Código', type: 'text' as const, required: true },
    { name: 'unitKey', label: 'Unidad', type: 'text' as const, required: true, placeholder: 'm, kg, hr, etc.' },
  ];

  const resourceOptions = resources.map(r => ({
    label: `${r.name} (${r.unit}) - $${r.price}`,
    value: r.id,
  }));

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 dark:text-white">APUs</h1>
          <p className="text-gray-600 dark:text-gray-400 mt-2">
            Análisis de Precios Unitarios - Combina recursos con cantidades
          </p>
        </div>
        <button
          onClick={() => {
            setSelectedApu(null);
            setShowModal(true);
          }}
          className="flex items-center gap-2 px-6 py-2 bg-primary-600 hover:bg-primary-700 text-white rounded-lg font-medium transition-colors"
        >
          <Plus size={20} />
          Nuevo APU
        </button>
      </div>

      {/* Table */}
      <Card>
        <div className="p-6">
          <Table<ApuWithResources>
            data={apus}
            columns={[
              { header: 'Código', accessor: 'key', sortable: true },
              { header: 'Unidad', accessor: 'unitKey' },
              {
                header: 'Componentes',
                accessor: (row) => row.components?.length || 0,
              },
              {
                header: 'Costo Directo',
                accessor: (row) => `$${row.directCost?.toFixed(2) || '0.00'}`,
              },
            ]}
            rowKey="id"
            loading={loading}
            actions={(row) => (
              <div className="flex items-center gap-2">
                <button
                  onClick={() => setSelectedApu(row)}
                  className="p-2 text-blue-600 hover:bg-blue-50 dark:hover:bg-blue-900 rounded-lg transition-colors"
                  title="Ver detalles"
                >
                  <Eye size={16} />
                </button>
              </div>
            )}
          />
        </div>
      </Card>

      {/* Modal para crear */}
      <Modal
        isOpen={showModal && !selectedApu}
        onClose={() => setShowModal(false)}
        title="Nuevo APU"
      >
        <Form
          fields={formFields}
          onSubmit={handleCreate}
          onCancel={() => setShowModal(false)}
          submitLabel="Crear"
        />
      </Modal>

      {/* Modal para ver detalles y editar componentes */}
      <Modal
        isOpen={!!selectedApu}
        onClose={() => setSelectedApu(null)}
        title={`APU: ${selectedApu?.key}`}
        size="lg"
      >
        <div className="space-y-6">
          <div>
            <h3 className="font-semibold text-lg mb-3">Componentes</h3>
            <div className="space-y-2 mb-4">
              {selectedApu?.components?.length ? (
                selectedApu.components.map(comp => (
                  <div key={comp.id} className="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-800 rounded-lg">
                    <div>
                      <p className="font-medium">{comp.id}</p>
                      <p className="text-sm text-gray-600 dark:text-gray-400">
                        Cantidad: {comp.quantity} | Total: ${comp.total.toFixed(2)}
                      </p>
                    </div>
                    <button
                      onClick={() => handleRemoveComponent(comp.id)}
                      className="p-2 text-red-600 hover:bg-red-50 dark:hover:bg-red-900 rounded-lg"
                    >
                      <Trash2 size={16} />
                    </button>
                  </div>
                ))
              ) : (
                <p className="text-gray-500 dark:text-gray-400">Sin componentes</p>
              )}
            </div>

            {/* Add component form */}
            <div className="border-t border-gray-200 dark:border-gray-800 pt-4">
              <h4 className="font-semibold mb-3">Agregar Componente</h4>
              <AddComponentForm
                resources={resources}
                onAdd={handleAddComponent}
              />
            </div>
          </div>

          <div className="border-t border-gray-200 dark:border-gray-800 pt-4">
            <p className="text-sm text-gray-600 dark:text-gray-400">
              <span className="font-semibold">Costo Directo Total:</span> ${selectedApu?.directCost?.toFixed(2)}
            </p>
          </div>
        </div>
      </Modal>
    </div>
  );
};

interface AddComponentFormProps {
  resources: types.Resource[];
  onAdd: (resourceId: string, quantity: number) => void;
}

const AddComponentForm: React.FC<AddComponentFormProps> = ({ resources, onAdd }) => {
  const [resourceId, setResourceId] = React.useState('');
  const [quantity, setQuantity] = React.useState(1);

  const handleSubmit = () => {
    if (resourceId) {
      onAdd(resourceId, quantity);
      setResourceId('');
      setQuantity(1);
    }
  };

  return (
    <div className="space-y-3">
      <div>
        <label className="block text-sm font-medium mb-1">Recurso</label>
        <select
          value={resourceId}
          onChange={(e) => setResourceId(e.target.value)}
          className="w-full px-3 py-2 border border-gray-200 dark:border-gray-800 rounded-lg bg-white dark:bg-gray-900 text-gray-900 dark:text-white focus:ring-2 focus:ring-primary-500"
        >
          <option value="">Selecciona un recurso</option>
          {resources.map(r => (
            <option key={r.id} value={r.id}>
              {r.name} ({r.unit})
            </option>
          ))}
        </select>
      </div>
      <div>
        <label className="block text-sm font-medium mb-1">Cantidad</label>
        <input
          type="number"
          value={quantity}
          onChange={(e) => setQuantity(parseFloat(e.target.value) || 1)}
          min="0.01"
          step="0.01"
          className="w-full px-3 py-2 border border-gray-200 dark:border-gray-800 rounded-lg bg-white dark:bg-gray-900 text-gray-900 dark:text-white focus:ring-2 focus:ring-primary-500"
        />
      </div>
      <button
        onClick={handleSubmit}
        disabled={!resourceId}
        className="w-full px-4 py-2 bg-primary-600 hover:bg-primary-700 disabled:bg-gray-400 text-white rounded-lg font-medium transition-colors"
      >
        Agregar
      </button>
    </div>
  );
};
