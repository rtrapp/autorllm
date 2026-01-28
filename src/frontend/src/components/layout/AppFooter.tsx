import { Save } from "lucide-react"

export function AppFooter() {
  return (
    <footer className="h-8 border-t bg-muted/50 flex items-center px-4 text-xs text-muted-foreground justify-between shrink-0">
        <div className="flex items-center gap-2">
            <Save className="h-3 w-3" />
            <span>Salvo automaticamente às 14:30</span>
        </div>
        <div>
            Autosave ativo
        </div>
    </footer>
  )
}
