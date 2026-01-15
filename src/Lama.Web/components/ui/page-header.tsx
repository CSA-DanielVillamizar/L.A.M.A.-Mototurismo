import * as React from "react"
import { cn } from "@/lib/utils"

interface PageHeaderProps extends React.HTMLAttributes<HTMLDivElement> {
  heading: string
  description?: string
  children?: React.ReactNode
}

/**
 * PageHeader - Componente para encabezados de página
 * Uso: <PageHeader heading="Título" description="Descripción opcional" />
 */
export function PageHeader({
  heading,
  description,
  className,
  children,
  ...props
}: PageHeaderProps) {
  return (
    <div className={cn("space-y-4 pb-8 pt-6", className)} {...props}>
      <div className="flex items-center justify-between">
        <div className="space-y-1">
          <h1 className="text-3xl font-bold tracking-tight">{heading}</h1>
          {description && (
            <p className="text-muted-foreground">{description}</p>
          )}
        </div>
        {children && <div className="flex items-center space-x-2">{children}</div>}
      </div>
    </div>
  )
}
