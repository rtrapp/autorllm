import { Button } from "@/components/ui/button";
import { Link } from "react-router-dom";
import { Plus } from "lucide-react";

export default function ProjectsList() {
  return (
    <div className="container mx-auto p-8">
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-3xl font-bold">Meus Projetos</h1>
        <Button>
          <Plus className="mr-2 h-4 w-4" /> Novo Projeto
        </Button>
      </div>
      
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {/* Mock Project Card */}
        <div className="border rounded-lg p-6 shadow-sm hover:shadow-md transition-shadow">
          <h2 className="text-xl font-semibold mb-2">Crônicas de Aethel</h2>
          <p className="text-muted-foreground mb-4">
            Uma fantasia épica sobre uma vila esquecida e um inverno antigo.
          </p>
          <div className="flex justify-between items-center text-sm text-muted-foreground mb-4">
            <span>2 Capítulos</span>
            <span>Atualizado há 2h</span>
          </div>
          <Link to="/projects/123">
            <Button variant="outline" className="w-full">
              Abrir
            </Button>
          </Link>
        </div>
      </div>
    </div>
  );
}
