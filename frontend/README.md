# OpenAPU Frontend

Interfaz gráfica moderna y amigable para OpenAPU - Sistema de Gestión de Presupuestos de Obra.

## Características

- 🎨 Diseño moderno y responsivo con Tailwind CSS
- 🌓 Tema claro/oscuro automático
- 📊 Gestión completa de recursos, APUs, conceptos y presupuestos
- 📈 Tableros intuitivos con datos en tiempo real
- 💾 Importación/Exportación de datos en CSV
- 🔄 Respaldo y restauración de base de datos
- ⚡ Rápido con Vite y React 18
- 🔌 API RESTful completamente integrada

## Estructura del Proyecto

```
src/
├── components/        # Componentes reutilizables
├── pages/             # Páginas principales
├── services/          # Servicios API
├── store/             # Estado global
├── types/             # Tipos TypeScript
```

## Instalación

```bash
npm install
cp .env.example .env.local
```

## Desarrollo

```bash
npm run dev
# La aplicación estará disponible en http://localhost:5173
```

## Build

```bash
npm run build
npm run preview
```

## Tecnologías

- React 18 + TypeScript
- Vite + Tailwind CSS
- React Router + Zustand
- Axios + Lucide Icons

## Licencia

MIT
