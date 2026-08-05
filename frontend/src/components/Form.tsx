import React from 'react';

export interface FormField {
  name: string;
  label: string;
  type: 'text' | 'number' | 'email' | 'textarea' | 'select' | 'password';
  placeholder?: string;
  required?: boolean;
  options?: { value: string | number; label: string }[];
  className?: string;
}

interface FormProps {
  fields: FormField[];
  onSubmit: (values: Record<string, any>) => void;
  onCancel?: () => void;
  submitLabel?: string;
  loading?: boolean;
  initialValues?: Record<string, any>;
  className?: string;
}

export const Form: React.FC<FormProps> = ({
  fields,
  onSubmit,
  onCancel,
  submitLabel = 'Guardar',
  loading = false,
  initialValues = {},
  className = '',
}) => {
  const [values, setValues] = React.useState(initialValues);
  const [errors, setErrors] = React.useState<Record<string, string>>({});

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value, type } = e.target;
    const finalValue = type === 'number' ? parseFloat(value) || '' : value;
    setValues((prev) => ({ ...prev, [name]: finalValue }));
    if (errors[name]) {
      setErrors((prev) => {
        const newErrors = { ...prev };
        delete newErrors[name];
        return newErrors;
      });
    }
  };

  const validate = () => {
    const newErrors: Record<string, string> = {};

    fields.forEach((field) => {
      if (field.required && !values[field.name]) {
        newErrors[field.name] = `${field.label} es requerido`;
      }
    });

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (validate()) {
      onSubmit(values);
    }
  };

  return (
    <form onSubmit={handleSubmit} className={`space-y-6 ${className}`}>
      {fields.map((field) => (
        <div key={field.name}>
          <label htmlFor={field.name} className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
            {field.label}
            {field.required && <span className="text-red-500">*</span>}
          </label>

          {field.type === 'textarea' ? (
            <textarea
              id={field.name}
              name={field.name}
              value={values[field.name] || ''}
              onChange={handleChange}
              placeholder={field.placeholder}
              className={`w-full px-4 py-2 border border-gray-200 dark:border-gray-800 rounded-lg bg-white dark:bg-gray-900 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-600 focus:outline-none focus:ring-2 focus:ring-primary-500 ${
                errors[field.name] ? 'border-red-500 ring-2 ring-red-500' : ''
              }`}
              rows={4}
            />
          ) : field.type === 'select' ? (
            <select
              id={field.name}
              name={field.name}
              value={values[field.name] || ''}
              onChange={handleChange}
              className={`w-full px-4 py-2 border border-gray-200 dark:border-gray-800 rounded-lg bg-white dark:bg-gray-900 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-primary-500 ${
                errors[field.name] ? 'border-red-500 ring-2 ring-red-500' : ''
              }`}
            >
              <option value="">Selecciona una opción</option>
              {field.options?.map((opt) => (
                <option key={opt.value} value={opt.value}>
                  {opt.label}
                </option>
              ))}
            </select>
          ) : (
            <input
              id={field.name}
              type={field.type}
              name={field.name}
              value={values[field.name] || ''}
              onChange={handleChange}
              placeholder={field.placeholder}
              className={`w-full px-4 py-2 border border-gray-200 dark:border-gray-800 rounded-lg bg-white dark:bg-gray-900 text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-600 focus:outline-none focus:ring-2 focus:ring-primary-500 ${
                errors[field.name] ? 'border-red-500 ring-2 ring-red-500' : ''
              }`}
            />
          )}

          {errors[field.name] && (
            <p className="text-red-500 text-sm mt-1">{errors[field.name]}</p>
          )}
        </div>
      ))}

      <div className="flex justify-end gap-3 pt-4">
        {onCancel && (
          <button
            type="button"
            onClick={onCancel}
            className="px-6 py-2 border border-gray-200 dark:border-gray-800 rounded-lg font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-900 transition-colors"
          >
            Cancelar
          </button>
        )}
        <button
          type="submit"
          disabled={loading}
          className="px-6 py-2 bg-primary-600 hover:bg-primary-700 disabled:bg-gray-400 text-white rounded-lg font-medium transition-colors flex items-center gap-2"
        >
          {loading && <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white"></div>}
          {submitLabel}
        </button>
      </div>
    </form>
  );
};
