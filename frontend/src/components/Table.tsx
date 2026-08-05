import React from 'react';
import { ChevronUp, ChevronDown } from 'lucide-react';

export interface TableColumn<T> {
  header: string;
  accessor: keyof T | ((row: T) => React.ReactNode);
  sortable?: boolean;
  className?: string;
}

interface TableProps<T> {
  data: T[];
  columns: TableColumn<T>[];
  rowKey: keyof T;
  onRowClick?: (row: T) => void;
  actions?: (row: T) => React.ReactNode;
  loading?: boolean;
}

export function Table<T>({
  data,
  columns,
  rowKey,
  onRowClick,
  actions,
  loading = false,
}: TableProps<T>) {
  const [sortBy, setSortBy] = React.useState<keyof T | null>(null);
  const [sortOrder, setSortOrder] = React.useState<'asc' | 'desc'>('asc');

  const handleSort = (column: TableColumn<T>) => {
    if (!column.sortable) return;

    if (sortBy === column.accessor) {
      setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
    } else {
      setSortBy(column.accessor as keyof T);
      setSortOrder('asc');
    }
  };

  const sortedData = React.useMemo(() => {
    if (!sortBy) return data;

    return [...data].sort((a, b) => {
      const aValue = a[sortBy];
      const bValue = b[sortBy];

      if (aValue < bValue) return sortOrder === 'asc' ? -1 : 1;
      if (aValue > bValue) return sortOrder === 'asc' ? 1 : -1;
      return 0;
    });
  }, [data, sortBy, sortOrder]);

  return (
    <div className="overflow-x-auto">
      <table className="w-full text-sm">
        <thead>
          <tr className="border-b border-gray-200 dark:border-gray-800 bg-gray-50 dark:bg-gray-900">
            {columns.map((column, idx) => (
              <th
                key={idx}
                className={`px-6 py-3 text-left font-semibold text-gray-700 dark:text-gray-300 cursor-pointer hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors ${
                  column.className || ''
                }`}
                onClick={() => handleSort(column)}
              >
                <div className="flex items-center gap-2">
                  {column.header}
                  {column.sortable && sortBy === column.accessor && (
                    sortOrder === 'asc' ? <ChevronUp size={16} /> : <ChevronDown size={16} />
                  )}
                </div>
              </th>
            ))}
            {actions && <th className="px-6 py-3 text-right font-semibold text-gray-700 dark:text-gray-300">Acciones</th>}
          </tr>
        </thead>
        <tbody>
          {loading ? (
            <tr>
              <td colSpan={columns.length + (actions ? 1 : 0)} className="px-6 py-8 text-center">
                <div className="flex justify-center">
                  <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
                </div>
              </td>
            </tr>
          ) : sortedData.length === 0 ? (
            <tr>
              <td
                colSpan={columns.length + (actions ? 1 : 0)}
                className="px-6 py-8 text-center text-gray-500 dark:text-gray-400"
              >
                No hay datos para mostrar
              </td>
            </tr>
          ) : (
            sortedData.map((row) => (
              <tr
                key={String(row[rowKey])}
                className="border-b border-gray-200 dark:border-gray-800 hover:bg-gray-50 dark:hover:bg-gray-900 transition-colors cursor-pointer"
                onClick={() => onRowClick?.(row)}
              >
                {columns.map((column, idx) => (
                  <td key={idx} className={`px-6 py-4 ${column.className || ''}`}>
                    {typeof column.accessor === 'function'
                      ? column.accessor(row)
                      : String(row[column.accessor] || '-')}
                  </td>
                ))}
                {actions && (
                  <td className="px-6 py-4 text-right" onClick={(e) => e.stopPropagation()}>
                    {actions(row)}
                  </td>
                )}
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
}
