import { ProjectFormDialog } from "./ProjectFormDialog";

interface NewProjectDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function NewProjectDialog({ open, onOpenChange }: NewProjectDialogProps) {
  return (
    <ProjectFormDialog 
      open={open} 
      onOpenChange={onOpenChange} 
      mode="create"
    />
  );
}
