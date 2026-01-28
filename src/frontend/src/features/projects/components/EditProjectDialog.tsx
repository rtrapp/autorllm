import { ProjectFormDialog } from "./ProjectFormDialog";
import type { Project } from "../hooks/useProjects";

interface EditProjectDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  project: Project;
  onSuccess?: () => void;
}

export function EditProjectDialog({ open, onOpenChange, project, onSuccess }: EditProjectDialogProps) {
  return (
    <ProjectFormDialog 
      open={open} 
      onOpenChange={onOpenChange} 
      mode="edit"
      project={project}
      onSuccess={onSuccess}
    />
  );
}
