# 🚀 OpenAPU Frontend - Guía Rápida de Inicio

## Resumen Ejecutivo

Se ha desarrollado una **aplicación web moderna, completa y visualmente atractiva** para OpenAPU que proporciona una interfaz gráfica intuitiva para gestionar presupuestos de obra.

### ✨ Lo que hace especial esta aplicación:

1. **Diseño Moderno** - UI/UX profesional con Tailwind CSS
2. **Responsive** - Funciona en móvil, tablet y desktop
3. **Tema Oscuro** - Soporte para dark mode automático
4. **Completa** - Todas las funcionalidades de OpenAPU en la web
5. **Rápida** - Compilada con Vite para máximo rendimiento
6. **Type-Safe** - 100% TypeScript para robustez

---

## 🎯 Funcionalidades Principales

### Dashboard Ejecutivo
- 📊 Estadísticas en tiempo real
- 🔄 Actualizaciones automáticas
- 🎯 Accesos rápidos

### Gestión de Recursos
- ➕ Crear materiales, mano de obra, equipos
- ✏️ Editar precios y propiedades
- 🗑️ Eliminar o desactivar
- 📋 Tabla interactiva con búsqueda

### Análisis de Precios Unitarios (APU)
- 🧮 Crear análisis combinando recursos
- 📝 Agregar componentes con cantidades
- 💰 Cálculos automáticos de costos

### Conceptos Presupuestarios
- 🎁 Crear conceptos a partir de APUs
- % Configurar márgenes (utilidad, indirectos, financiamiento)
- 📈 Cálculo automático de precio final

### Gestión de Presupuestos
- 📄 Crear presupuestos completos
- 📍 Agregar partidas (conceptos)
- 💵 Totales automáticos
- 📥 Exportar a CSV

### Importación/Exportación
- 📤 Importar recursos desde CSV
- 📥 Exportar presupuestos
- 💾 Plantillas descargables

### Backup y Recuperación
- 🔄 Crear respaldos de base de datos
- 📦 Restaurar desde respaldos previos
- 🛡️ Protección de datos

---

## ⚡ Inicio Rápido

### Paso 1: Instalar
```bash
cd frontend
npm install
```

### Paso 2: Configurar
```bash
cp .env.example .env.local
# El API debe estar en http://localhost:5080
```

### Paso 3: Ejecutar
```bash
npm run dev
```

### Paso 4: Abrir
```
http://localhost:5173
```

---

## 📦 Stack Tecnológico

| Tecnología | Versión | Propósito |
|-----------|---------|----------|
| React | 18+ | UI Framework |
| TypeScript | Latest | Type Safety |
| Vite | Latest | Build Tool |
| Tailwind CSS | Latest | Styling |
| React Router | Latest | Navegación |
| Zustand | Latest | Estado Global |
| Axios | Latest | HTTP Client |
| Lucide Icons | Latest | Iconografía |

---

## 🏗️ Estructura de Carpetas

```
frontend/
├── src/
│   ├── components/      → Layout, Card, Table, Form, Modal
│   ├── pages/          → Dashboard, Resources, Apus, Concepts, Budgets, Settings
│   ├── services/       → Cliente API
│   ├── store/          → Estado global (Zustand)
│   ├── types/          → Tipos TypeScript
│   └── App.tsx         → Enrutador principal
├── package.json        → Dependencias
├── tailwind.config.js  → Temas y colores
├── vite.config.ts      → Configuración build
└── tsconfig.json       → Configuración TypeScript
```

---

## 🎨 Características Visuales

### Tema Oscuro/Claro
- Soporte automático según preferencia del sistema
- Toggle manual en la interfaz
- Persistencia en localStorage

### Navegación Intuitiva
- Sidebar con opciones principales
- Breadcrumbs en cada página
- Accesos rápidos en dashboard
- Notificaciones inteligentes

### Tablas Interactivas
- Ordenamiento por columnas
- Selección de filas
- Acciones contextuales
- Carga progresiva

### Formularios Validados
- Validación en tiempo real
- Mensajes de error claros
- Campos requeridos marcados
- Tipos de entrada específicos

### Modales Elegantes
- Animaciones suaves
- Backdrop oscuro
- Cierre con ESC
- Contenido desplazable

---

## 📊 Ejemplo de Flujo de Trabajo

### Crear un Presupuesto Completo

1. **Dashboard** → Ver estadísticas del sistema
2. **Recursos** → Crear materiales y mano de obra
   - Material A: $100/m
   - Mano de obra: $50/hr
3. **APUs** → Crear análisis combinando recursos
   - APU "Excavación": 2m × Material A + 1hr × Mano de obra
   - Costo directo: $200
4. **Conceptos** → Crear concepto con márgenes
   - Basado en APU "Excavación"
   - Utilidad: 20%
   - Costos indirectos: 10%
   - Precio final: $264
5. **Presupuestos** → Crear presupuesto final
   - Agregar 100 unidades del concepto
   - Total: $26,400
   - Exportar como CSV

---

## 🔗 Integración con OpenAPU

La aplicación se conecta directamente con la API OpenAPU:

```
Frontend (React)  ←→  API (localhost:5080)  ←→  SQLite Database
```

### Endpoints Utilizados

| Método | Endpoint | Función |
|--------|----------|---------|
| GET | /resources | Listar recursos |
| POST | /resources | Crear recurso |
| GET | /apus | Listar APUs |
| POST | /apus | Crear APU |
| GET | /concepts | Listar conceptos |
| POST | /concepts | Crear concepto |
| GET | /budgets | Listar presupuestos |
| POST | /budgets | Crear presupuesto |
| GET | /system/status | Estado del sistema |
| POST | /imports/resources.csv | Importar recursos |
| GET | /exports/budgets.csv | Exportar presupuestos |
| POST | /database/backup | Crear respaldo |
| POST | /database/restore | Restaurar base de datos |

---

## 📈 Producción

### Build para Producción
```bash
npm run build
# Genera carpeta 'dist/' lista para desplegar
```

### Opciones de Despliegue

#### Opción A: Con OpenAPU (Recomendado)
```bash
# Copiar dist/* a src/OpenAPU.Api/wwwroot/
npm run build
cp -r dist/* ../src/OpenAPU.Api/wwwroot/
```

#### Opción B: Servidor Web Independiente
```bash
# Desplegar en Nginx, Apache, Vercel, etc.
npm run build
# Subir contenido de 'dist/' a hosting
```

#### Opción C: Docker
```bash
docker build -t openapu-frontend .
docker run -p 3000:80 openapu-frontend
```

---

## 🐛 Troubleshooting

### "Cannot connect to API"
```bash
# Verificar que OpenAPU está corriendo
cd ../src/OpenAPU.Api
dotnet run
# Debe estar en http://localhost:5080
```

### "Build fails"
```bash
# Limpiar y reinstalar
rm -rf node_modules package-lock.json
npm install
npm run build
```

### "Dark theme not working"
```bash
# Verificar localStorage
localStorage.setItem('darkMode', 'true')
# Recargar página
```

---

## 📚 Documentación Adicional

Para documentación técnica detallada, ver:
- `frontend/DOCUMENTATION.md` - Guía completa del desarrollo
- `frontend/README.md` - Información del proyecto

---

## ✅ Checklist de Verificación

- [ ] Node.js 16+ instalado
- [ ] OpenAPU API corriendo en puerto 5080
- [ ] `npm install` completado
- [ ] `.env.local` configurado
- [ ] `npm run dev` iniciado
- [ ] Accedible en http://localhost:5173
- [ ] Dashboard carga sin errores
- [ ] Puedo crear un recurso
- [ ] Puedo crear un APU
- [ ] Puedo crear un concepto
- [ ] Puedo crear un presupuesto

---

## 🎉 ¡Listo!

La aplicación está lista para usar. Explora las diferentes secciones y comienza a crear presupuestos profesionales con OpenAPU.

### Próximos Pasos
1. Crea algunos recursos básicos
2. Construye un APU
3. Crea un concepto
4. Genera tu primer presupuesto
5. Exporta a CSV

---

**Versión:** 1.0.0
**Última actualización:** Agosto 2024
**Autor:** Claude AI
**Licencia:** MIT
