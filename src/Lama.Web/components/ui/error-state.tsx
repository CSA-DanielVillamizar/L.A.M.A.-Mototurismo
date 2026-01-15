import * as React from "react"
import { AlertCircle, XCircle } from "lucide-react"
import { cn } from "@/lib/utils"
import { Alert, AlertDescription, AlertTitle } from "./alert"
import { Button } from "./button"

interface ErrorStateProps extends React.HTMLAttributes<HTMLDivElement> {
  title?: string
  message: string
  error?: Error
  onRetry?: () => void
  variant?: "default" | "destructive"
}

/**
 * ErrorState - Componente para mostrar estados de error
 * Uso: <ErrorState message="Error al cargar datos" onRetry={() => refetch()} />
 */
export function ErrorState({
  title = "Error",
  message,
  error,
  onRetry,
  variant = "destructive",
  className,
  ...props
}: ErrorStateProps) {
  return (
    <div className={cn("py-8", className)} {...props}>
      <Alert variant={variant}>
        <AlertCircle className="h-4 w-4" />
        <AlertTitle>{title}</AlertTitle>
        <AlertDescription>
          <p>{message}</p>
          {error && (
            <details className="mt-2 text-xs opacity-70">
              <summary className="cursor-pointer hover:opacity-100">
                Detalles técnicos
              </summary>
              <pre className="mt-2 whitespace-pre-wrap rounded bg-muted p-2">
                {error.message}
              </pre>
            </details>
          )}
          {onRetry && (
            <Button
              variant="outline"
              size="sm"
              onClick={onRetry}
              className="mt-4"
            >
              Reintentar
            </Button>
          )}
        </AlertDescription>
      </Alert>
    </div>
  )
}

interface InlineErrorProps {
  message: string
}

/**
 * InlineError - Componente para errores inline en formularios
 * Uso: <InlineError message="Campo requerido" />
 */
export function InlineError({ message }: InlineErrorProps) {
  return (
    <p className="text-sm font-medium text-destructive flex items-center gap-1.5 mt-1.5">
      <XCircle className="h-3.5 w-3.5" />
      {message}
    </p>
  )
}
