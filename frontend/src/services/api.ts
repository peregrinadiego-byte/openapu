import axios, { AxiosInstance } from 'axios';
import * as types from '../types';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5080';

const api: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Resources
export const resourceAPI = {
  getAll: async () => {
    const response = await api.get<types.Resource[]>('/resources');
    return response.data;
  },

  getById: async (id: string) => {
    const response = await api.get<types.Resource>(`/resources/${id}`);
    return response.data;
  },

  create: async (command: types.CreateResourceCommand) => {
    const response = await api.post<types.Resource>('/resources', command);
    return response.data;
  },

  update: async (id: string, data: Partial<types.Resource>) => {
    const response = await api.put<types.Resource>(`/resources/${id}`, {
      name: data.name,
      price: data.price,
      isActive: data.status === types.ResourceStatus.Active,
    });
    return response.data;
  },
};

// APUs
export const apuAPI = {
  getAll: async () => {
    const response = await api.get<types.Apu[]>('/apus');
    return response.data;
  },

  getById: async (id: string) => {
    const response = await api.get<types.Apu>(`/apus/${id}`);
    return response.data;
  },

  create: async (command: types.CreateApuCommand) => {
    const response = await api.post<types.Apu>('/apus', command);
    return response.data;
  },

  addComponent: async (apuId: string, resourceId: string, quantity: number) => {
    const response = await api.post<types.Apu>(
      `/apus/${apuId}/components`,
      { resourceId, quantity }
    );
    return response.data;
  },

  updateComponent: async (apuId: string, componentId: string, quantity: number) => {
    const response = await api.put<types.Apu>(
      `/apus/${apuId}/components/${componentId}`,
      { quantity }
    );
    return response.data;
  },

  removeComponent: async (apuId: string, componentId: string) => {
    const response = await api.delete<types.Apu>(
      `/apus/${apuId}/components/${componentId}`
    );
    return response.data;
  },

  refreshPrices: async (apuId: string) => {
    const response = await api.post<types.Apu>(`/apus/${apuId}/refresh-prices`);
    return response.data;
  },
};

// Concepts
export const conceptAPI = {
  getAll: async () => {
    const response = await api.get<types.Concept[]>('/concepts');
    return response.data;
  },

  getById: async (id: string) => {
    const response = await api.get<types.Concept>(`/concepts/${id}`);
    return response.data;
  },

  create: async (command: types.CreateConceptCommand) => {
    const response = await api.post<types.Concept>('/concepts', command);
    return response.data;
  },

  updatePercentages: async (
    id: string,
    indirectCost: number,
    financing: number,
    profit: number,
    additionalCharges: number
  ) => {
    const response = await api.put<types.Concept>(
      `/concepts/${id}/percentages`,
      { indirectCost, financing, profit, additionalCharges }
    );
    return response.data;
  },
};

// Budgets
export const budgetAPI = {
  getAll: async () => {
    const response = await api.get<types.Budget[]>('/budgets');
    return response.data;
  },

  getById: async (id: string) => {
    const response = await api.get<types.Budget>(`/budgets/${id}`);
    return response.data;
  },

  create: async (command: types.CreateBudgetCommand) => {
    const response = await api.post<types.Budget>('/budgets', command);
    return response.data;
  },

  addItem: async (budgetId: string, conceptId: string, quantity: number) => {
    const response = await api.post<types.Budget>(
      `/budgets/${budgetId}/items`,
      { conceptId, quantity }
    );
    return response.data;
  },

  updateItem: async (budgetId: string, itemId: string, quantity: number) => {
    const response = await api.put<types.Budget>(
      `/budgets/${budgetId}/items/${itemId}`,
      { quantity }
    );
    return response.data;
  },

  removeItem: async (budgetId: string, itemId: string) => {
    const response = await api.delete<types.Budget>(
      `/budgets/${budgetId}/items/${itemId}`
    );
    return response.data;
  },

  refreshPrices: async (budgetId: string) => {
    const response = await api.post<types.Budget>(
      `/budgets/${budgetId}/refresh-prices`
    );
    return response.data;
  },
};

// System
export const systemAPI = {
  getStatus: async () => {
    const response = await api.get<types.SystemStatus>('/system/status');
    return response.data;
  },

  getHealth: async () => {
    const response = await api.get('/health');
    return response.data;
  },
};

// Export/Import
export const ioAPI = {
  exportBudgetsCSV: async () => {
    const response = await api.get('/exports/budgets.csv', {
      responseType: 'blob',
    });
    return response.data;
  },

  exportApusCSV: async () => {
    const response = await api.get('/exports/apus.csv', {
      responseType: 'blob',
    });
    return response.data;
  },

  importResourcesCSV: async (file: File) => {
    const formData = new FormData();
    formData.append('file', file);
    const response = await api.post('/imports/resources.csv', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },

  getResourcesTemplate: async () => {
    const response = await api.get('/imports/resources/template.csv', {
      responseType: 'blob',
    });
    return response.data;
  },

  backup: async () => {
    const response = await api.get('/database/backup', {
      responseType: 'blob',
    });
    return response.data;
  },

  restore: async (file: File) => {
    const formData = new FormData();
    formData.append('file', file);
    const response = await api.post('/database/restore', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },
};

export default api;
