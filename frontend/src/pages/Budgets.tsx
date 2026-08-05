import React, { useEffect, useState } from 'react';
import { Plus, Edit2, Trash2, Eye, Download } from 'lucide-react';
import { Card, Table, Modal, Form } from '../components';
import * as types from '../types';
import { budgetAPI, conceptAPI, ioAPI } from '../services/api';
import { useAppStore } from '../store/useAppStore';

export const Budgets: React.FC = () => {
  const [budgets, setBudgets] = useState<types.Budget[]>([]);
  const [concepts, setConcepts] = useState<types.Concept[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [selectedBudget, setSelectedBudget] = useState<types.Budget | null>(null);
  const { showNotification } = useAppStore();

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [budgetsData, conceptsData] = await Promise.all([
        budgetAPI.getAll(),
        conceptAPI.getAll(),
      ]);
      setBudgets(budgetsData);
      setConcepts(conceptsData);
    } catch (error) {
      showNotification('Error al cargar presupuestos', 'error');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (values: Record<string, any>) => {
    try {
      await budgetAPI.create(values as types.CreateBudgetCommand);
      showNotification('Presupuesto creado exitosamente', 'success');
      setShowModal(false);
      fetchData();
    } catch (error) {
      showNotification('Error al crear presupuesto', 'error');
    }
  };

  const handleAddItem = async (conceptId: string, quantity: number) => {
    try {
      if (selectedBudget) {
        const updated = await budgetAPI.addItem(selectedBudget.id, conceptId, quantity);
        showNotification('Partida agregada', 'success');
        setBudgets(budgets.map(b => b.id === selectedBudget.id ? updated : b));
        setSelectedBudget(updated);
      }
    } catch (error) {
      showNotification('Error al agregar partida', 'error');
    }
  };

  const handleRemoveItem = async (itemId: string) => {
    try {
      if (selectedBudget) {
        const updated = await budgetAPI.removeItem(selectedBudget.id, itemId);
        showNotification('Partida removida', 'success');
        setBudgets(budgets.map(b => b.id === selectedBudget.id ? updated : b));
        setSelectedBudget(updated);
      }
    } catch (error) {
      showNotification('Error al remover partida', 'error');
    }
  };

  const handleExport = async () => {
    try {
      const blob = await ioAPI.exportBudgetsCSV();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `presupuestos-${new Date().toISOString().split('T')[0]}.csv`;
      a.click();
      showNotification('Presupuestos exportados', 'success');
    } catch (error) {
      showNotification('Error al exportar', 'error');
    }
  };

  const formFields = [
    { name: 'key', label: 'Código', type: 'text' as const, required: true },
    { name: 'name', label: 'Nombre', type: 'text' as const, required: true },
  ];

  const conceptOptions = concepts.map(c => ({
    label: `${c.name} - $${c.unitPrice.toFixed(2)}`,
    value: c.id,
  }));

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 dark:text-white">Presupuestos</h1>
          <p className="text-gray-600 dark:text-gray-400 mt-2">
            Crea y gestiona presupuestos de obra
          </p>
        </div>
        <div className="flex gap-2">
          <button
            onClick={handleExport}
            className="flex items-center gap-2 px-6 py-2 bg-gray-200 dark:bg-gray-800 hover:bg-gray-300 dark:hover:bg-gray-700 text-gray-900 dark:text-white rounded-lg font-medium transition-colors"
          >
            <Download size={20} />
            Exportar CSV
          </button>
          <button
            onClick={() => {
              setSelectedBudget(null);
              setShowModal(true);
            }}
            className="flex items-center gap-2 px-6 py-2 bg-primary-600 hover:bg-primary-700 text-white rounded-lg font-medium transition-colors"
          >
            <Plus size={20} />
            Nuevo Presupuesto
          </button>
        </div>
      </div>

      {/* Table */}
      <Card>
        <div className="p-6">
          <Table<types.Budget>
            data={budgets}
            columns={[
              { header: 'Código', accessor: 'key', sortable: true },
              { header: 'Nombre', accessor: 'name', sortable: true },
              {
                header: 'Partidas',
                accessor: (row) => row.items?.length || 0,
              },
              {
                header: 'Total',
                accessor: (row) => `$${row.total.toFixed(2)}`,
              },
            ]}
            rowKey="id"
            loading={loading}
            actions={(row) => (
              <button
                onClick={() => setSelectedBudget(row)}
                className="p-2 text-blue-600 hover:bg-blue-50 dark:hover:bg-blue-900 rounded-lg transition-colors"
                title="Ver detalles"
              >
                <Eye size={16} />
              </button>
            )}
          />
        </div>
      </Card>

      {/* Create Modal */}
      <Modal
        isOpen={showModal}
        onClose={() => setShowModal(false)}
        title="Nuevo Presupuesto"
      >
        <Form
          fields={formFields}
          onSubmit={handleCreate}
          onCancel={() => setShowModal(false)}
          submitLabel="Crear"
        />
      </Modal>

      {/* Detail Modal */}
      <Modal
        isOpen={!!selectedBudget}
        onClose={() => setSelectedBudget(null)}
        title={`Presupuesto: ${selectedBudget?.name}`}
        size="lg"
      >
        <div className="space-y-6">
          <div className="p-4 bg-primary-50 dark:bg-primary-900 rounded-lg">
            <p className="text-sm text-gray-600 dark:text-gray-300">Total del Presupuesto</p>
            <p className="text-3xl font-bold text-primary-700 dark:text-primary-200">
              ${selectedBudget?.total.toFixed(2)}
            </p>
          </div>

          <div>
            <h3 className="font-semibold text-lg mb-3">Partidas</h3>
            <div className="space-y-2 mb-4">
              {selectedBudget?.items?.length ? (
                selectedBudget.items.map(item => (
                  <div key={item.id} className="p-3 bg-gray-50 dark:bg-gray-800 rounded-lg">
                    <div className="flex items-start justify-between mb-2">
                      <div>
                        <p className="font-medium">{item.concept?.name || 'Concepto'}</p>
                        <p className="text-sm text-gray-600 dark:text-gray-400">
                          Cantidad: {item.quantity}
                        </p>
                      </div>
                      <button
                        onClick={() => handleRemoveItem(item.id)}
                        className="p-2 text-red-600 hover:bg-red-50 dark:hover:bg-red-900 rounded-lg"
                      >
                        <Trash2 size={16} />
                      </button>
                    </div>
                    <div className="flex justify-between text-sm">
                      <span className="text-gray-600 dark:text-gray-400">
                        ${item.unitPrice.toFixed(2)} × {item.quantity}
                      </span>
                      <span className="font-semibold">${item.total.toFixed(2)}</span>
                    </div>
                  </div>
                ))
              ) : (
                <p className="text-gray-500 dark:text-gray-400">Sin partidas</p>
              )}
            </div>

            {/* Add item form */}
            <div className="border-t border-gray-200 dark:border-gray-800 pt-4">
              <h4 className="font-semibold mb-3">Agregar Partida</h4>
              <AddItemForm
                concepts={concepts}
                onAdd={handleAddItem}
              />
            </div>
          </div>
        </div>
      </Modal>
    </div>
  );
};

interface AddItemFormProps {
  concepts: types.Concept[];
  onAdd: (conceptId: string, quantity: number) => void;
}

const AddItemForm: React.FC<AddItemFormProps> = ({ concepts, onAdd }) => {
  const [conceptId, setConceptId] = React.useState('');
  const [quantity, setQuantity] = React.useState(1);

  const handleSubmit = () => {
    if (conceptId) {
      onAdd(conceptId, quantity);
      setConceptId('');
      setQuantity(1);
    }
  };

  return (
    <div className="space-y-3">
      <div>
        <label className="block text-sm font-medium mb-1">Concepto</label>
        <select
          value={conceptId}
          onChange={(e) => setConceptId(e.target.value)}
          className="w-full px-3 py-2 border border-gray-200 dark:border-gray-800 rounded-lg bg-white dark:bg-gray-900 text-gray-900 dark:text-white focus:ring-2 focus:ring-primary-500"
        >
          <option value="">Selecciona un concepto</option>
          {concepts.map(c => (
            <option key={c.id} value={c.id}>
              {c.name} (${c.unitPrice.toFixed(2)})
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
        disabled={!conceptId}
        className="w-full px-4 py-2 bg-primary-600 hover:bg-primary-700 disabled:bg-gray-400 text-white rounded-lg font-medium transition-colors"
      >
        Agregar
      </button>
    </div>
  );
};
