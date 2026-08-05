// Resources
export interface Resource {
  id: string;
  key: string;
  name: string;
  type: ResourceType;
  unit: string;
  price: number;
  status: ResourceStatus;
}

export enum ResourceType {
  Material = 'Material',
  Labor = 'Labor',
  Equipment = 'Equipment',
  Tool = 'Tool',
  Auxiliary = 'Auxiliary'
}

export enum ResourceStatus {
  Active = 'Active',
  Inactive = 'Inactive'
}

// APU Components
export interface ApuComponent {
  id: string;
  resourceId: string;
  resource?: Resource;
  quantity: number;
  total: number;
}

export interface Apu {
  id: string;
  key: string;
  unitKey: string;
  components: ApuComponent[];
  directCost: number;
}

// Concepts
export interface Concept {
  id: string;
  key: string;
  name: string;
  unit: string;
  directCost: number;
  indirectCost: number;
  financing: number;
  profit: number;
  additionalCharges: number;
  unitPrice: number;
  apu: Apu;
}

// Budgets
export interface BudgetItem {
  id: string;
  conceptId: string;
  concept?: Concept;
  quantity: number;
  unitPrice: number;
  total: number;
}

export interface Budget {
  id: string;
  key: string;
  name: string;
  items: BudgetItem[];
  total: number;
}

// System Status
export interface SystemStatus {
  name: string;
  version: string;
  database: string;
  resources: number;
  apus: number;
  concepts: number;
  budgets: number;
  checkedAtUtc: string;
}

// API Responses
export interface CreateResourceCommand {
  key: string;
  name: string;
  type: ResourceType;
  unit: string;
  price: number;
}

export interface CreateApuCommand {
  key: string;
  unitKey: string;
}

export interface CreateConceptCommand {
  key: string;
  name: string;
  unitKey: string;
  apuId: string;
}

export interface CreateBudgetCommand {
  key: string;
  name: string;
}
