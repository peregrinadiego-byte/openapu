import React, { useEffect, useState } from 'react';
import { Plus, Edit2, Trash2 } from 'lucide-react';
import { Card, Table, Modal, Form } from '../components';
import * as types from '../types';
import { resourceAPI } from '../services/api';
import { useAppStore } from '../store/useAppStore';

export const Resources: React.FC = () => {
  const [resources, setResources] = useState<types.Resource[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const { showNotification } = useAppStore();

  useEffect(() => {
    fetchResources();
  }, []);

  const fetchResources = async () => {
    try {
      setLoading(true);
      const data = await resourceAPI.getAll();
      setResources(data);
    } catch (error) {
      showNotification('Error al cargar recursos', 'error');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (values: Record<string, any>) => {
    try {
      await resourceAPI.create(values as types.CreateResourceCommand);
      showNotification('Recurso creado exitosamente', 'success');
      setShowModal(false);
      fetchResources();
    } catch (error) {
      showNotification('Error al crear recurso', 'error');
    }
  };

  const handleUpdate = async (values: Record<string, any>) => {
    try {
      if (editingId) {
        await resourceAPI.update(editingId, values);
        showNotification('Recurso actualizado exitosamente', 'success');
        setEditingId(null);
        setShowModal(false);
        fetchResources();
      }
    } catch (error) {
      showNotification('Error al actualizar recurso', 'error');
    }
  };

  const handleDelete = async (id: string) => {
    if (confirm('¿Estás seguro de que deseas eliminar este recurso?')) {
      try {
        // Note: API might not support delete, so we'll deactivate instead
        const resource = resources.find(r => r.id === id);
        if (resource) {
          await resourceAPI.update(id, {
            ...resource,
            status: types.ResourceStatus.Inactive,
          });
          showNotification('Recurso eliminado', 'success');
          fetchResources();
        }
      } catch (error) {
        showNotification('Error al eliminar recurso', 'error');
      }
    }
  };

  const resourceTypes = Object.values(types.ResourceType).map(type => ({
    label: type,
    value: type,
  }));

  const formFields = [
    { name: 'key', label: 'Código', type: 'text' as const, required: true },
    { name: 'name', label: 'Nombre', type: 'text' as const, required: true },
    {
      name: 'type',
      label: 'Tipo',
      type: 'select' as const,
      required: true,
      options: resourceTypes,
    },
    { name: 'unit', label: 'Unidad', type: 'text' as const, required: true, placeholder: 'm, kg, hr, etc.' },
    { name: 'price', label: 'Precio', type: 'number' as const, required: true },
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 dark:text-white">Recursos</h1>
          <p className="text-gray-600 dark:text-gray-400 mt-2">
            Gestiona materiales, mano de obra, equipo y otras recursos
          </p>
        </div>
        <button
          onClick={() => {
            setEditingId(null);
            setShowModal(true);
          }}
          className="flex items-center gap-2 px-6 py-2 bg-primary-600 hover:bg-primary-700 text-white rounded-lg font-medium transition-colors"
        >
          <Plus size={20} />
          Nuevo Recurso
        </button>
      </div>

      {/* Table */}
      <Card>
        <div className="p-6">
          <Table<types.Resource>
            data={resources}
            columns={[
              { header: 'Código', accessor: 'key', sortable: true },
              { header: 'Nombre', accessor: 'name', sortable: true },
              { header: 'Tipo', accessor: 'type', sortable: true },
              { header: 'Unidad', accessor: 'unit' },
              {
                header: 'Precio',
                accessor: (row) => `$${row.price.toFixed(2)}`,
              },
              {
                header: 'Estado',
                accessor: (row) => (
                  <span
                    className={`px-3 py-1 rounded-full text-sm font-medium ${
                      row.status === types.ResourceStatus.Active
                        ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
                        : 'bg-gray-100 text-gray-800 dark:bg-gray-800 dark:text-gray-200'
                    }`}
                  >
                    {row.status === types.ResourceStatus.Active ? 'Activo' : 'Inactivo'}
                  </span>
                ),
              },
            ]}
            rowKey="id"
            loading={loading}
            actions={(row) => (
              <div className="flex items-center gap-2">
                <button
                  onClick={() => {
                    setEditingId(row.id);
                    setShowModal(true);
                  }}
                  className="p-2 text-blue-600 hover:bg-blue-50 dark:hover:bg-blue-900 rounded-lg transition-colors"
                  title="Editar"
                >
                  <Edit2 size={16} />
                </button>
                <button
                  onClick={() => handleDelete(row.id)}
                  className="p-2 text-red-600 hover:bg-red-50 dark:hover:bg-red-900 rounded-lg transition-colors"
                  title="Eliminar"
                >
                  <Trash2 size={16} />
                </button>
              </div>
            )}
          />
        </div>
      </Card>

      {/* Modal */}
      <Modal
        isOpen={showModal}
        onClose={() => {
          setShowModal(false);
          setEditingId(null);
        }}
        title={editingId ? 'Editar Recurso' : 'Nuevo Recurso'}
      >
        <Form
          fields={formFields}
          onSubmit={editingId ? handleUpdate : handleCreate}
          onCancel={() => {
            setShowModal(false);
            setEditingId(null);
          }}
          submitLabel={editingId ? 'Actualizar' : 'Crear'}
          initialValues={
            editingId ? resources.find(r => r.id === editingId) || {} : {}
          }
        />
      </Modal>
    </div>
  );
};
