import { BrainstormChat } from '@/components/brainstorm/BrainstormChat';
import { useNavigate } from 'react-router-dom';

export default function BrainstormPage() {
  const navigate = useNavigate();

  const handleClose = () => {
    navigate('/projects');
  };

  return (
    <div className="flex h-screen bg-background">
      <div className="flex-1 flex items-center justify-center bg-secondary/10">
        <div className="text-center max-w-2xl px-8">
          <h1 className="text-4xl font-bold mb-4">Crie seu Livro com IA</h1>
          <p className="text-lg text-muted-foreground mb-8">
            Descreva sua ideia e deixe a inteligência artificial te ajudar a estruturar sua história.
            Responda algumas perguntas e receba um outline completo para começar a escrever.
          </p>
          <div className="bg-card border rounded-lg p-6 text-left">
            <h3 className="font-semibold mb-3">Como funciona:</h3>
            <ol className="space-y-2 text-sm text-muted-foreground">
              <li>1. Descreva sua ideia de livro no chat ao lado</li>
              <li>2. A IA fará perguntas para entender melhor sua visão</li>
              <li>3. Responda às perguntas para refinar sua história</li>
              <li>4. Receba um outline estruturado com capítulos e personagens</li>
              <li>5. Salve o projeto e comece a escrever!</li>
            </ol>
          </div>
        </div>
      </div>
      <BrainstormChat onClose={handleClose} />
    </div>
  );
}
