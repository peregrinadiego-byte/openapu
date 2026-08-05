import React, { useEffect, useState } from 'react';
import { Plus, Edit2 } from 'lucide-react';
import { Card, Table, Modal, Form } from '../components';
import * as types from '../types';
import { conceptAPI, apuAPI } from '../services/api';
import { useAppStore } from '../store/useAppStore';

export const Concepts: React.FC = () => {
  const [concepts, setConcepts] = useState<types.Concept[]>([]);
  const [apus, setApus] = useState<types.Apu[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [selectedConcept, setSelectedConcept] = useState<types.Concept | null>(null);
  const { showNotification } = useAppStore();

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [conceptsData, apusData] = await Promise.all([
        conceptAPI.getAll(),
        apuAPI.getAll(),
      ]);
      setConcepts(conceptsData);
      setApus(apusData);
    } catch (error) {
      showNotification('Error al cargar conceptos', 'error');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (values: Record<string, any>) => {
    try {
      await conceptAPI.create(values as types.CreateConceptCommand);
      showNotification('Concepto creado exitosamente', 'success');
      setShowModal(false);
      fetchData();
    } catch (error) {
      showNotification('Error al crear concepto', 'error');
    }
  };

  const handleUpdatePercentages = async (values: Record<string, any>) => {
    try {
      if (editingId) {
        await conceptAPI.updatePercentages(
          editingId,
          values.indirectCost,
          values.financing,
          values.profit,
          values.additionalCharges
        );
        showNotification('Concepto actualizado', 'success');
        setEditingId(null);
        setShowModal(false);
        fetchData();
      }
    } catch (error) {
      showNotification('Error al actualizar concepto', 'error');
    }
  };

  const apuOptions = apus.map(a => ({
    label: `${a.key} (${a.unitKey})`,
    value: a.id,
  }));

  const createFormFields = [
    { name: 'key', label: 'Código', type: 'text' as const, required: true },
    { name: 'name', label: 'Nombre', type: 'text' as const, required: true },
    { name: 'unitKey', label: 'Unidad', type: 'text' as const, required: true },
    {
      name: 'apuId',
      label: 'APU',
      type: 'select' as const,
      required: true,
      options: apuOptions,
    },
  ];

  const percentageFields = [
    { name: 'indirectCost', label: 'Costos Indirectos (%)', type: 'number' as const, required: true },
    { name: 'financing', label: 'Financiamiento (%)', type: 'number' as const, required: true },
    { name: 'profit', label: 'Utilidad (%)', type: 'number' as const, required: true },
    { name: 'additionalCharges', label: 'Cargos Adicionales (%)', type: 'number' as const, required: true },
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 dark:text-white">Conceptos</h1>
          <p className="text-gray-600 dark:text-gray-400 mt-2">
            Crea conceptos con APUs y porcentajes de margen
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
          Nuevo Concepto
        </button>
      </div>

      {/* Table */}
      <Card>
        <div className="p-6">
          <Table<types.Concept>
            data={concepts}
            columns={[
              { header: 'Código', accessor: 'key', sortable: true },
              { header: 'Nombre', accessor: 'name', sortable: true },
              { header: 'Unidad', accessor: 'unit' },
              {
                header: 'Costo Directo',
                accessor: (row) => `$${row.directCost.toFixed(2)}`,
              },
              {
                header: 'Precio Unitario',
                accessor: (row) => `$${row.unitPrice.toFixed(2)}`,
              },
            ]}
            rowKey="id"
            loading={loading}
            onRowClick={(row) => setSelectedConcept(row)}
            actions={(row) => (
              <button
                onClick={() => {
                  setEditingId(row.id);
                  setShowModal(true);
                }}
                className="p-2 text-blue-600 hover:bg-blue-50 dark:hover:bg-blue-900 rounded-lg transition-colors"
                title="Editar porcentajes"
              >
                <Edit2 size={16} />
              </button>
            )}
          />
        </div>
      </Card>

      {/* Detail Modal */}
      {selectedConcept && (
        <Modal
          isOpen={!!selectedConcept}
          onClose={() => setSelectedConcept(null)}
          title={`Concepto: ${selectedConcept.name}`}
          size="lg"
        >
          <div className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <p className="text-sm text-gray-600 dark:text-gray-400">Código</p>
                <p className="font-semibold">{selectedConcept.key}</p>
              </div>
              <div>
                <p className="text-sm text-gray-600 dark:text-gray-400">Unidad</p>
                <p className="font-semibold">{selectedConcept.unit}</p>
              </div>
              <div>
                <p className="text-sm text-gray-600 dark:text-gray-400">Costo Directo</p>
                <p className="font-semibold">${selectedConcept.directCost.toFixed(2)}</p>
              </div>
              <div>
                <p className="text-sm text-gray-600 dark:text-gray-400">Precio Unitario</p>
                <p className="font-semibold text-primary-600">${selectedConcept.unitPrice.toFixed(2)}</p>
              </div>
            </div>

            <div className="border-t border-gray-200 dark:border-gray-800 pt-4">
              <h3 className="font-semibold mb-3">Porcentajes</h3>
              <div className="grid grid-cols-2 gap-3 text-sm">
                <div>
                  <span className="text-gray-600 dark:text-gray-400">Costos Indirectos:</span>
                  <p className="font-medium">{(selectedConcept.indirectCost * 100).toFixed(2)}%</p>
                </div>
                <div>
                  <span className="text-gray-600 dark:text-gray-400">Financiamiento:</span>
                  <p className="font-medium">{(selectedConcept.financing * 100).toFixed(2)}%</p>
                </div>
                <div>
                  <span className="text-gray-600 dark:text-gray-400">Utilidad:</span>
                  <p className="font-medium">{(selectedConcept.profit * 100).toFixed(2)}%</p>
                </div>
                <div>
                  <span className="text-gray-600 dark:text-gray-400">Cargos Adicionales:</span>
                  <p className="font-medium">{(selectedConcept.additionalCharges * 100).toFixed(2)}%</p>
                </div>
              </div>
            </div>
          </div>
        </Modal>
      )}

      {/* Create Modal */}
      <Modal
        isOpen={showModal && !editingId}
        onClose={() => setShowModal(false)}
        title="Nuevo Concepto"
      >
        <Form
          fields={createFormFields}
          onSubmit={handleCreate}
          onCancel={() => setShowModal(false)}
          submitLabel="Crear"
        />
      </Modal>

      {/* Edit Modal */}
      <Modal
        isOpen={!!editingId}
        onClose={() => setEditingId(null)}
        title="Editar Porcentajes"
      >
        <Form
          fields={percentageFields}
          onSubmit={handleUpdatePercentages}
          onCancel={() => setEditingId(null)}
          submitLabel="Actualizar"
          initialValues={
            concepts.find(c => c.id === editingId) || {}
          }
        />
      </Modal>
    </div>
  );
};
