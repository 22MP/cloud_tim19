import type { ButtonHTMLAttributes, InputHTMLAttributes, ReactNode } from "react";

// ── Button ────────────────────────────────────────────────────────────────────
type ButtonVariant = "primary" | "secondary" | "danger" | "ghost" | "warning";
type ButtonSize = "sm" | "md" | "lg";

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: ButtonVariant;
  size?: ButtonSize;
  loading?: boolean;
  icon?: ReactNode;
}

const variantClasses: Record<ButtonVariant, string> = {
  primary: "bg-amber-500 hover:bg-amber-600 text-white shadow-sm",
  secondary: "bg-white hover:bg-gray-50 text-gray-700 border border-gray-300 shadow-sm",
  danger: "bg-red-500 hover:bg-red-600 text-white shadow-sm",
  ghost: "hover:bg-amber-50 text-amber-700",
  warning: "bg-yellow-400 hover:bg-yellow-500 text-white shadow-sm",
};

const sizeClasses: Record<ButtonSize, string> = {
  sm: "px-3 py-1.5 text-sm",
  md: "px-4 py-2 text-sm",
  lg: "px-6 py-2.5 text-base",
};

export const Button = ({
  variant = "primary",
  size = "md",
  loading = false,
  icon,
  children,
  disabled,
  className = "",
  ...props
}: ButtonProps) => (
  <button
    disabled={disabled || loading}
    className={`
      inline-flex items-center gap-2 font-medium rounded-lg transition-all duration-150
      disabled:opacity-50 disabled:cursor-not-allowed
      ${variantClasses[variant]} ${sizeClasses[size]} ${className}
    `}
    {...props}
  >
    {loading ? (
      <svg className="animate-spin h-4 w-4" fill="none" viewBox="0 0 24 24">
        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
      </svg>
    ) : icon ? (
      <span className="w-4 h-4 flex items-center justify-center">{icon}</span>
    ) : null}
    {children}
  </button>
);

// ── Input ─────────────────────────────────────────────────────────────────────
interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
  icon?: ReactNode;
}

export const Input = ({ label, error, icon, className = "", ...props }: InputProps) => (
  <div className="flex flex-col gap-1">
    {label && <label className="text-sm font-medium text-gray-700">{label}</label>}
    <div className="relative">
      {icon && (
        <span className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 w-4 h-4">
          {icon}
        </span>
      )}
      <input
        className={`
          w-full border rounded-lg py-2 text-sm outline-none transition
          focus:ring-2 focus:ring-amber-400 focus:border-transparent
          ${error ? "border-red-400 bg-red-50" : "border-gray-300 bg-white"}
          ${icon ? "pl-9 pr-4" : "px-4"}
          ${className}
        `}
        {...props}
      />
    </div>
    {error && <span className="text-xs text-red-500">{error}</span>}
  </div>
);

// ── Select ────────────────────────────────────────────────────────────────────
interface SelectProps extends React.SelectHTMLAttributes<HTMLSelectElement> {
  label?: string;
  error?: string;
  options: { value: string | number; label: string }[];
}

export const Select = ({ label, error, options, className = "", ...props }: SelectProps) => (
  <div className="flex flex-col gap-1">
    {label && <label className="text-sm font-medium text-gray-700">{label}</label>}
    <select
      className={`
        w-full border rounded-lg px-4 py-2 text-sm outline-none transition
        focus:ring-2 focus:ring-amber-400 focus:border-transparent bg-white
        ${error ? "border-red-400" : "border-gray-300"}
        ${className}
      `}
      {...props}
    >
      {options.map((o) => (
        <option key={o.value} value={o.value}>
          {o.label}
        </option>
      ))}
    </select>
    {error && <span className="text-xs text-red-500">{error}</span>}
  </div>
);

// ── Textarea ──────────────────────────────────────────────────────────────────
interface TextareaProps extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
  label?: string;
  error?: string;
}

export const Textarea = ({ label, error, className = "", ...props }: TextareaProps) => (
  <div className="flex flex-col gap-1">
    {label && <label className="text-sm font-medium text-gray-700">{label}</label>}
    <textarea
      className={`
        w-full border rounded-lg px-4 py-2 text-sm outline-none transition resize-none
        focus:ring-2 focus:ring-amber-400 focus:border-transparent
        ${error ? "border-red-400 bg-red-50" : "border-gray-300 bg-white"}
        ${className}
      `}
      rows={3}
      {...props}
    />
    {error && <span className="text-xs text-red-500">{error}</span>}
  </div>
);

// ── Badge ─────────────────────────────────────────────────────────────────────
type BadgeVariant = "green" | "red" | "yellow" | "amber" | "purple" | "blue" | "gray";

const badgeColors: Record<BadgeVariant, string> = {
  green: "bg-green-100 text-green-700",
  red: "bg-red-100 text-red-700",
  yellow: "bg-yellow-100 text-yellow-700",
  amber: "bg-amber-100 text-amber-700",
  purple: "bg-purple-100 text-purple-700",
  blue: "bg-blue-100 text-blue-700",
  gray: "bg-gray-100 text-gray-600",
};

export const Badge = ({ label, variant = "gray" }: { label: string; variant?: BadgeVariant }) => (
  <span className={`px-2.5 py-0.5 rounded-full text-xs font-medium ${badgeColors[variant]}`}>
    {label}
  </span>
);

// ── Alert ─────────────────────────────────────────────────────────────────────
type AlertVariant = "error" | "success" | "warning" | "info";

const alertStyles: Record<AlertVariant, string> = {
  error: "bg-red-50 border-red-200 text-red-700",
  success: "bg-green-50 border-green-200 text-green-700",
  warning: "bg-yellow-50 border-yellow-200 text-yellow-700",
  info: "bg-blue-50 border-blue-200 text-blue-700",
};

export const Alert = ({ message, variant = "error" }: { message: string; variant?: AlertVariant }) => (
  <div className={`border rounded-lg px-4 py-3 text-sm ${alertStyles[variant]}`}>{message}</div>
);

// ── Card ──────────────────────────────────────────────────────────────────────
export const Card = ({
  children,
  className = "",
}: {
  children: ReactNode;
  className?: string;
}) => (
  <div className={`bg-white rounded-2xl shadow-sm border border-gray-100 ${className}`}>
    {children}
  </div>
);

// ── Modal ─────────────────────────────────────────────────────────────────────
interface ModalProps {
  open: boolean;
  onClose: () => void;
  title: string;
  children: ReactNode;
  maxWidth?: string;
}

export const Modal = ({ open, onClose, title, children, maxWidth = "max-w-lg" }: ModalProps) => {
  if (!open) return null;
  return (
    <div className="fixed inset-0 flex items-center justify-center p-4" style={{ zIndex: 10000 }}>
      <div className="absolute inset-0 bg-black/40 backdrop-blur-sm" onClick={onClose} />
      <div className={`relative bg-white rounded-2xl shadow-2xl w-full ${maxWidth} max-h-[90vh] overflow-y-auto`}>
        <div className="flex items-center justify-between p-6 border-b">
          <h2 className="text-lg font-bold text-gray-800">{title}</h2>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600 transition text-2xl leading-none"
          >
            ×
          </button>
        </div>
        <div className="p-6">{children}</div>
      </div>
    </div>
  );
};

// ── Spinner ───────────────────────────────────────────────────────────────────
export const Spinner = ({ className = "" }: { className?: string }) => (
  <div className={`flex items-center justify-center py-12 ${className}`}>
    <svg className="animate-spin h-8 w-8 text-amber-500" fill="none" viewBox="0 0 24 24">
      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
    </svg>
  </div>
);

// ── Empty State ───────────────────────────────────────────────────────────────
export const EmptyState = ({
  icon,
  title,
  description,
  action,
}: {
  icon: ReactNode;
  title: string;
  description?: string;
  action?: ReactNode;
}) => (
  <div className="flex flex-col items-center justify-center py-16 text-center">
    <div className="text-5xl mb-4 opacity-30">{icon}</div>
    <h3 className="text-gray-600 font-semibold text-lg">{title}</h3>
    {description && <p className="text-gray-400 text-sm mt-1 max-w-xs">{description}</p>}
    {action && <div className="mt-6">{action}</div>}
  </div>
);

// ── Toast ─────────────────────────────────────────────────────────────────────
interface ToastItem {
  id: number;
  message: string;
  type: "success" | "error" | "warning" | "info";
}

const toastStyles = {
  success: "bg-green-600",
  error: "bg-red-600",
  warning: "bg-yellow-500",
  info: "bg-blue-600",
};

const toastIcons = {
  success: "✓",
  error: "✕",
  warning: "⚠",
  info: "ℹ",
};

export const ToastContainer = ({
  toasts,
  onRemove,
}: {
  toasts: ToastItem[];
  onRemove: (id: number) => void;
}) => (
  <div className="fixed bottom-4 right-4 z-50 flex flex-col gap-2">
    {toasts.map((t) => (
      <div
        key={t.id}
        className={`${toastStyles[t.type]} text-white px-4 py-3 rounded-xl shadow-lg flex items-center gap-3 min-w-64 animate-slide-up`}
      >
        <span className="font-bold">{toastIcons[t.type]}</span>
        <span className="text-sm flex-1">{t.message}</span>
        <button onClick={() => onRemove(t.id)} className="opacity-70 hover:opacity-100 text-lg leading-none">
          ×
        </button>
      </div>
    ))}
  </div>
);

// ── Confirm Dialog ────────────────────────────────────────────────────────────
export const ConfirmDialog = ({
  open,
  onClose,
  onConfirm,
  title,
  message,
  confirmLabel = "Delete",
  variant = "danger",
}: {
  open: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title: string;
  message: string;
  confirmLabel?: string;
  variant?: ButtonVariant;
}) => (
  <Modal open={open} onClose={onClose} title={title} maxWidth="max-w-sm">
    <p className="text-gray-600 text-sm mb-6">{message}</p>
    <div className="flex gap-3 justify-end">
      <Button variant="secondary" onClick={onClose}>
        Cancel
      </Button>
      <Button variant={variant} onClick={() => { onConfirm(); onClose(); }}>
        {confirmLabel}
      </Button>
    </div>
  </Modal>
);