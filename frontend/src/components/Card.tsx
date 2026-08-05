import React from 'react';

interface CardProps {
  children: React.ReactNode;
  className?: string;
  onClick?: () => void;
}

export const Card: React.FC<CardProps> = ({ children, className = '', onClick }) => {
  return (
    <div
      className={`bg-white dark:bg-gray-900 rounded-lg border border-gray-200 dark:border-gray-800 shadow-sm hover:shadow-md transition-shadow ${className}`}
      onClick={onClick}
    >
      {children}
    </div>
  );
};

interface StatCardProps {
  label: string;
  value: string | number;
  icon?: React.ReactNode;
  color?: 'blue' | 'green' | 'purple' | 'amber';
  trend?: { value: number; isPositive: boolean };
}

const colorClasses = {
  blue: 'bg-blue-50 dark:bg-blue-900 text-blue-700 dark:text-blue-200',
  green: 'bg-green-50 dark:bg-green-900 text-green-700 dark:text-green-200',
  purple: 'bg-purple-50 dark:bg-purple-900 text-purple-700 dark:text-purple-200',
  amber: 'bg-amber-50 dark:bg-amber-900 text-amber-700 dark:text-amber-200',
};

export const StatCard: React.FC<StatCardProps> = ({
  label,
  value,
  icon,
  color = 'blue',
  trend,
}) => {
  return (
    <Card className="p-6">
      <div className="flex items-start justify-between">
        <div className="flex-1">
          <p className="text-sm font-medium text-gray-600 dark:text-gray-400">{label}</p>
          <p className="text-3xl font-bold mt-2">{value}</p>
          {trend && (
            <p
              className={`text-sm font-medium mt-2 ${
                trend.isPositive
                  ? 'text-green-600 dark:text-green-400'
                  : 'text-red-600 dark:text-red-400'
              }`}
            >
              {trend.isPositive ? '↑' : '↓'} {Math.abs(trend.value)}% desde último período
            </p>
          )}
        </div>
        {icon && <div className={`p-3 rounded-lg ${colorClasses[color]}`}>{icon}</div>}
      </div>
    </Card>
  );
};
