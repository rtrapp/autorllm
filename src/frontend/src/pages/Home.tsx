import { Button } from "@/components/ui/button";
import { Link } from "react-router-dom";

export default function Home() {
  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-background">
      <h1 className="text-4xl font-bold mb-8">Autor LLM</h1>
      <p className="text-xl text-muted-foreground mb-8">
        Seu assistente de escrita com IA local.
      </p>
      <Link to="/projects">
        <Button size="lg">Meus Projetos</Button>
      </Link>
    </div>
  );
}
