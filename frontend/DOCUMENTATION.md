# OpenAPU Frontend - Documentación Completa

## 📋 Índice
1. [Visión General](#visión-general)
2. [Estructura del Proyecto](#estructura-del-proyecto)
3. [Guía de Instalación](#guía-de-instalación)
4. [Páginas y Funcionalidades](#páginas-y-funcionalidades)
5. [Componentes](#componentes)
6. [Servicios API](#servicios-api)
7. [Estado Global](#estado-global)
8. [Estilos y Temas](#estilos-y-temas)
9. [Deployment](#deployment)

---

## Visión General

**OpenAPU Frontend** es una aplicación web moderna construida con React 18, TypeScript y Tailwind CSS. Proporciona una interfaz gráfica intuitiva y visualmente atractiva para gestionar presupuestos de obra, recursos, análisis de precios unitarios (APU) y conceptos presupuestarios.

### Características Principales

✅ **Diseño Responsivo** - Funciona perfectamente en móviles, tablets y desktop
✅ **Tema Oscuro/Claro** - Tema adaptativo con preferencias del usuario
✅ **Gestión de Recursos** - CRUD completo para materiales, mano de obra, equipos
✅ **APU Management** - Crear y administrar análisis de precios unitarios
✅ **Conceptos** - Gestionar conceptos con porcentajes de margen
✅ **Presupuestos** - Sistema completo de presupuestos con cálculos automáticos
✅ **Importación/Exportación** - CSV para recursos y presupuestos
✅ **Respaldo y Restauración** - Backup de base de datos completa

---

## Estructura del Proyecto

```
frontend/
├── src/
│   ├── components/          # Componentes reutilizables
│   │   ├── Layout.tsx      # Estructura principal (sidebar + header + content)
│   │   ├── Card.tsx        # Componentes Card y StatCard
│   │   ├── Table.tsx       # Tabla genérica con ordenamiento
│   │   ├── Form.tsx        # Formulario genérico validado
│   │   ├── Modal.tsx       # Modal elegante
│   │   └── index.ts        # Exports
│   │
│   ├── pages/              # Páginas de la aplicación
│   │   ├── Dashboard.tsx   # Vista principal con estadísticas
│   │   ├── Resources.tsx   # Gestión de recursos
│   │   ├── Apus.tsx        # Gestión de APUs
│   │   ├── Concepts.tsx    # Gestión de conceptos
│   │   ├── Budgets.tsx     # Gestión de presupuestos
│   │   └── Settings.tsx    # Configuración e importación/exportación
│   │
│   ├── services/
│   │   └── api.ts          # Cliente HTTP con todas las rutas
│   │
│   ├── store/
│   │   └── useAppStore.ts  # Estado global con Zustand
│   │
│   ├── types/
│   │   └── index.ts        # Tipos TypeScript
│   │
│   ├── App.tsx             # Componente raíz con routing
│   ├── main.tsx            # Entry point
│   └── index.css           # Estilos Tailwind
│
├── public/                 # Archivos estáticos
├── package.json           # Dependencias
├── tsconfig.json          # Configuración TypeScript
├── vite.config.ts         # Configuración Vite
├── tailwind.config.js     # Configuración Tailwind
├── postcss.config.js      # Configuración PostCSS
└── README.md              # README del proyecto
```

---

## Guía de Instalación

### Requisitos Previos
- Node.js 16+
- npm 7+
- OpenAPU API corriendo en http://localhost:5080

### Paso 1: Instalar Dependencias
```bash
cd frontend
npm install
```

### Paso 2: Configurar Entorno
```bash
cp .env.example .env.local
# Editar .env.local si la API está en diferente URL
```

### Paso 3: Iniciar Desarrollo
```bash
npm run dev
```

La aplicación estará disponible en **http://localhost:5173**

### Paso 4: Build para Producción
```bash
npm run build
npm run preview
```

Los archivos compilados estarán en `dist/`

---

## Páginas y Funcionalidades

### 🏠 Dashboard
**Ruta:** `/`

**Características:**
- Estadísticas en tiempo real (Recursos, APUs, Conceptos, Presupuestos)
- Estado del sistema (base de datos, versión)
- Accesos rápidos a funciones principales
- Información sobre el último chequeo de sistema

**Componentes:**
- StatCard para mostrar métricas
- Card para información del sistema
- Actualizaciones automáticas cada 10 segundos

---

### 📦 Recursos
**Ruta:** `/resources`

**Funcionalidades:**
- **Crear Recurso** - Nuevo material, mano de obra, equipo, herramienta o auxiliar
- **Listar Recursos** - Tabla con búsqueda y ordenamiento
- **Editar Recurso** - Modificar nombre, precio y estado
- **Eliminar Recurso** - Desactivar recurso (soft delete)

**Campos:**
- Código (Key)
- Nombre
- Tipo (Material, Labor, Equipment, Tool, Auxiliary)
- Unidad (m, kg, hr, etc.)
- Precio
- Estado (Activo/Inactivo)

**UI Destacada:**
- Tabla interactiva con colores para estado
- Modal para crear/editar
- Acciones rápidas (editar, eliminar)

---

### 📐 APUs (Análisis de Precios Unitarios)
**Ruta:** `/apus`

**Funcionalidades:**
- **Crear APU** - Nuevo análisis de precios
- **Listar APUs** - Tabla con componentes y costo directo
- **Agregar Componentes** - Seleccionar recursos y cantidades
- **Editar Cantidades** - Modificar cantidad de componentes
- **Eliminar Componentes** - Remover recursos del APU

**Cálculos Automáticos:**
- Costo directo = Σ(precio_recurso × cantidad)
- Actualización en tiempo real

**UI Destacada:**
- Modal con detalles de APU
- Tabla de componentes dentro del modal
- Formulario para agregar recursos

---

### 📚 Conceptos
**Ruta:** `/concepts`

**Funcionalidades:**
- **Crear Concepto** - A partir de un APU
- **Listar Conceptos** - Tabla con costo directo y precio unitario
- **Editar Porcentajes** - Ajustar márgenes:
  - Costos Indirectos (%)
  - Financiamiento (%)
  - Utilidad (%)
  - Cargos Adicionales (%)

**Cálculo de Precio Unitario:**
```
Precio Unitario = Costo Directo × (1 + % Indirectos + % Financiamiento + % Utilidad + % Cargos)
```

**UI Destacada:**
- Tabla comparativa de costos
- Modal con detalles completos
- Formulario de porcentajes validado

---

### 💰 Presupuestos
**Ruta:** `/budgets`

**Funcionalidades:**
- **Crear Presupuesto** - Nuevo proyecto
- **Listar Presupuestos** - Tabla con total y cantidad de partidas
- **Agregar Partidas** - Seleccionar conceptos y cantidades
- **Editar Cantidades** - Modificar cantidad de partidas
- **Eliminar Partidas** - Remover conceptos del presupuesto
- **Exportar CSV** - Descargar presupuestos en formato CSV

**Cálculos:**
- Total = Σ(precio_concepto × cantidad_partida)
- Actualización en tiempo real

**UI Destacada:**
- Modal expandido con presupuesto completo
- Resumen visual del total en tarjeta destacada
- Tabla de partidas con acciones

---

### ⚙️ Configuración
**Ruta:** `/settings`

**Funcionalidades:**

#### Importación de Recursos
- Descargar plantilla CSV
- Importar recursos desde archivo CSV
- Validación automática

#### Respaldo y Restauración
- **Crear Respaldo** - Descargar copia completa de BD
- **Restaurar** - Cargar respaldo anterior
- Confirmación de seguridad

#### Información de API
- URL base configurada
- Endpoints disponibles
- Notas técnicas

---

## Componentes

### Layout
El componente principal que envuelve toda la aplicación.

**Características:**
- Sidebar colapsable con navegación
- Header con notificaciones
- Tema oscuro/claro
- Indicador de estado

```tsx
<Layout>
  <YourPage />
</Layout>
```

### Card & StatCard
Componentes para mostrar información.

```tsx
<Card className="p-6">
  Contenido
</Card>

<StatCard
  label="Recursos"
  value={42}
  icon={<Package />}
  color="blue"
/>
```

### Table
Tabla genérica con TypeScript y funcionalidades avanzadas.

```tsx
<Table<Resource>
  data={resources}
  columns={[
    { header: 'Nombre', accessor: 'name', sortable: true }
  ]}
  rowKey="id"
  loading={loading}
  onRowClick={(row) => console.log(row)}
  actions={(row) => <button>Editar</button>}
/>
```

### Form
Formulario genérico con validación automática.

```tsx
<Form
  fields={[
    { name: 'name', label: 'Nombre', type: 'text', required: true }
  ]}
  onSubmit={(values) => console.log(values)}
  submitLabel="Guardar"
/>
```

### Modal
Modal elegante y accesible.

```tsx
<Modal
  isOpen={showModal}
  onClose={() => setShowModal(false)}
  title="Título del Modal"
  size="lg"
>
  Contenido del modal
</Modal>
```

---

## Servicios API

El archivo `services/api.ts` contiene todos los clientes para comunicarse con la API OpenAPU.

### Clientes Disponibles

**resourceAPI**
```ts
- getAll()
- getById(id)
- create(command)
- update(id, data)
```

**apuAPI**
```ts
- getAll()
- getById(id)
- create(command)
- addComponent(apuId, resourceId, quantity)
- updateComponent(apuId, componentId, quantity)
- removeComponent(apuId, componentId)
- refreshPrices(apuId)
```

**conceptAPI**
```ts
- getAll()
- getById(id)
- create(command)
- updatePercentages(id, ...)
```

**budgetAPI**
```ts
- getAll()
- getById(id)
- create(command)
- addItem(budgetId, conceptId, quantity)
- updateItem(budgetId, itemId, quantity)
- removeItem(budgetId, itemId)
- refreshPrices(budgetId)
```

**ioAPI**
```ts
- exportBudgetsCSV()
- exportApusCSV()
- importResourcesCSV(file)
- getResourcesTemplate()
- backup()
- restore(file)
```

**systemAPI**
```ts
- getStatus()
- getHealth()
```

---

## Estado Global

Usando **Zustand** para estado global simple y eficiente.

```ts
const { darkMode, toggleDarkMode } = useAppStore();
const { showNotification } = useAppStore();
const { isLoading, setLoading } = useAppStore();
```

**Estados:**
- `darkMode` - Tema actual (persistente)
- `notification` - Notificación actual
- `isLoading` - Indicador de carga global
- `systemStatus` - Estado del sistema

---

## Estilos y Temas

### Tailwind CSS
- Colores primarios personalizados
- Soporte completo para dark mode
- Componentes reutilizables
- Responsive design

### Variables de Color

```css
--primary-50: #f0f9ff
--primary-500: #0ea5e9
--primary-600: #0284c7
--primary-700: #0369a1
```

### Temas Soportados
- **Light Mode** - Colores claros y suave
- **Dark Mode** - Colores oscuros y contraste

El tema se persiste en localStorage.

---

## Deployment

### Opción 1: Servir con OpenAPU
1. Compilar con `npm run build`
2. Copiar contenido de `dist/` a `src/OpenAPU.Api/wwwroot/`
3. Compilar y desplegar OpenAPU

### Opción 2: Despliegue Independiente
1. Compilar con `npm run build`
2. Desplegar `dist/` en servidor web (Nginx, Apache, Vercel, etc.)
3. Configurar proxy a API OpenAPU

### Opción 3: Docker
```dockerfile
FROM node:18-alpine as build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

### Ambiente Variables para Producción
```bash
REACT_APP_API_URL=https://api.example.com
```

---

## Troubleshooting

### Error: Cannot find module
```bash
npm install
```

### CORS Error
- Verificar que OpenAPU tiene CORS habilitado
- Revisar `REACT_APP_API_URL` en `.env.local`

### Build Errors
```bash
rm -rf node_modules package-lock.json
npm install
npm run build
```

### API Not Responding
- Verificar que OpenAPU está corriendo en puerto 5080
- Verificar logs de OpenAPU
- Revisar conexión de red

---

## Roadmap Futuro

- [ ] Gráficos avanzados de presupuestos
- [ ] Reportes PDF
- [ ] Autenticación y usuarios
- [ ] Historial de cambios
- [ ] Comparación de presupuestos
- [ ] Búsqueda global
- [ ] Modo offline
- [ ] PWA (Progressive Web App)

---

## Soporte

Para reportar bugs o solicitar features, abrir un issue en el repositorio de GitHub.

---

**Última actualización:** Agosto 2024
**Versión:** 1.0.0
**Licencia:** MIT
