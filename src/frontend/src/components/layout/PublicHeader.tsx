import { Link, useLocation } from "react-router-dom";
import { Sparkles, Search } from "lucide-react";

export function PublicHeader() {
  const location = useLocation();

  return (
    <header className="h-16 border-b bg-background/80 backdrop-blur supports-[backdrop-filter]:bg-background/60 sticky top-0 z-50">
      <div className="container mx-auto px-4 h-full flex items-center justify-between">
        <div className="flex items-center gap-6">
          <Link to="/" className="font-bold text-xl flex items-center gap-2 tracking-tight hover:opacity-80 transition-opacity">
            <Sparkles className="text-primary" size={24} />
            <span>Neural Author</span>
          </Link>
          <nav className="hidden md:flex items-center gap-1">
            <Link
              to="/projects"
              className={`text-sm font-medium px-3 py-2 rounded-md transition-colors ${
                location.pathname === "/projects"
                  ? "bg-secondary text-foreground"
                  : "text-muted-foreground hover:bg-secondary/50"
              }`}
            >
              Projetos
            </Link>
            <Link
              to="/ideas"
              className={`text-sm font-medium px-3 py-2 rounded-md transition-colors ${
                location.pathname === "/ideas"
                  ? "bg-secondary text-foreground"
                  : "text-muted-foreground hover:bg-secondary/50"
              }`}
            >
              Ideias & Rascunhos
            </Link>
          </nav>
        </div>

        <div className="flex items-center gap-4">
          <div className="hidden md:flex relative items-center bg-secondary/50 rounded-md px-3 h-9 text-sm text-muted-foreground w-64 border border-transparent focus-within:border-primary/30 transition-all">
            <Search className="mr-2" size={16} />
            <span>Buscar projetos...</span>
          </div>

          <button className="h-9 w-9 rounded-full bg-primary/10 flex items-center justify-center text-primary font-semibold ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 hover:bg-primary/20 transition-colors">
            AU
          </button>
        </div>
      </div>
    </header>
  );
}
