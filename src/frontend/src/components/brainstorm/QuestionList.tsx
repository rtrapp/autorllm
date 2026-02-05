import { useState } from 'react';
import { CheckCircle2, Circle, ChevronRight, ChevronLeft } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import type { QuestionListComponent } from '@/types/ag-ui';

interface QuestionListProps {
  component: QuestionListComponent;
  onAnswer?: (questionId: string, answer: string) => void;
  onComplete?: (answers: Record<string, string>) => void;
}

export function QuestionList({ component, onAnswer, onComplete }: QuestionListProps) {
  const { questions, currentIndex = 0, showOneAtATime = true } = component;
  
  const [answers, setAnswers] = useState<Record<string, string>>({});
  const [activeQuestionIndex, setActiveQuestionIndex] = useState(currentIndex);
  const [currentAnswer, setCurrentAnswer] = useState('');

  const activeQuestion = questions[activeQuestionIndex];
  const isLastQuestion = activeQuestionIndex === questions.length - 1;
  const isFirstQuestion = activeQuestionIndex === 0;
  const hasAnswer = currentAnswer.trim().length > 0;

  const answeredCount = Object.keys(answers).length;
  const totalQuestions = questions.length;

  const handleNext = () => {
    if (!hasAnswer) return;

    // Save current answer
    const updatedAnswers = {
      ...answers,
      [activeQuestion.id]: currentAnswer.trim(),
    };
    setAnswers(updatedAnswers);

    if (onAnswer) {
      onAnswer(activeQuestion.id, currentAnswer.trim());
    }

    if (isLastQuestion) {
      // All questions answered, notify completion
      if (onComplete) {
        onComplete(updatedAnswers);
      }
    } else {
      // Move to next question
      setActiveQuestionIndex(activeQuestionIndex + 1);
      
      // Load existing answer if question was previously answered
      const nextQuestion = questions[activeQuestionIndex + 1];
      setCurrentAnswer(answers[nextQuestion.id] || '');
    }
  };

  const handlePrevious = () => {
    if (isFirstQuestion) return;

    // Save current answer before going back
    if (currentAnswer.trim()) {
      setAnswers({
        ...answers,
        [activeQuestion.id]: currentAnswer.trim(),
      });
    }

    setActiveQuestionIndex(activeQuestionIndex - 1);
    
    // Load previous answer
    const prevQuestion = questions[activeQuestionIndex - 1];
    setCurrentAnswer(answers[prevQuestion.id] || '');
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && (e.ctrlKey || e.metaKey)) {
      e.preventDefault();
      handleNext();
    }
  };

  if (showOneAtATime) {
    // Show one question at a time with navigation
    return (
      <Card className="w-full">
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle className="text-base font-medium">
              Pergunta {activeQuestionIndex + 1} de {totalQuestions}
            </CardTitle>
            <div className="text-sm text-muted-foreground">
              {answeredCount}/{totalQuestions} respondidas
            </div>
          </div>
          
          {/* Progress bar */}
          <div className="h-1.5 bg-muted rounded-full overflow-hidden mt-3">
            <div 
              className="h-full bg-primary transition-all duration-300"
              style={{ width: `${(answeredCount / totalQuestions) * 100}%` }}
            />
          </div>
        </CardHeader>
        
        <CardContent className="space-y-4">
          {/* Category badge */}
          <div className="inline-flex items-center px-2.5 py-1 rounded-md bg-primary/10 text-primary text-xs font-medium">
            {activeQuestion.category}
          </div>
          
          {/* Question text */}
          <p className="text-base font-medium leading-relaxed">
            {activeQuestion.text}
          </p>
          
          {/* Answer textarea */}
          <Textarea
            value={currentAnswer}
            onChange={(e) => setCurrentAnswer(e.target.value)}
            onKeyDown={handleKeyDown}
            placeholder="Digite sua resposta aqui..."
            className="min-h-[120px] resize-none"
            autoFocus
          />
          
          {/* Navigation buttons */}
          <div className="flex items-center justify-between gap-3 pt-2">
            <Button
              variant="outline"
              size="sm"
              onClick={handlePrevious}
              disabled={isFirstQuestion}
              className="gap-1.5"
            >
              <ChevronLeft className="h-4 w-4" />
              Anterior
            </Button>
            
            <div className="flex gap-1.5">
              {questions.map((_, index) => (
                <div
                  key={index}
                  className={`h-2 w-2 rounded-full transition-colors ${
                    index === activeQuestionIndex
                      ? 'bg-primary'
                      : answers[questions[index].id]
                      ? 'bg-primary/50'
                      : 'bg-muted'
                  }`}
                />
              ))}
            </div>
            
            <Button
              onClick={handleNext}
              disabled={!hasAnswer}
              size="sm"
              className="gap-1.5"
            >
              {isLastQuestion ? 'Concluir' : 'Próxima'}
              {!isLastQuestion && <ChevronRight className="h-4 w-4" />}
            </Button>
          </div>
          
          <p className="text-xs text-muted-foreground text-center">
            Dica: Pressione Ctrl+Enter para avançar
          </p>
        </CardContent>
      </Card>
    );
  }

  // Show all questions at once (fallback)
  return (
    <Card className="w-full">
      <CardHeader>
        <CardTitle className="text-base">Perguntas para expandir sua ideia</CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        {questions.map((question, index) => {
          const isAnswered = !!answers[question.id];
          
          return (
            <div key={question.id} className="space-y-2">
              <div className="flex items-start gap-2">
                {isAnswered ? (
                  <CheckCircle2 className="h-5 w-5 text-primary shrink-0 mt-0.5" />
                ) : (
                  <Circle className="h-5 w-5 text-muted-foreground shrink-0 mt-0.5" />
                )}
                <div className="flex-1">
                  <p className="font-medium text-sm">
                    {index + 1}. {question.category}
                  </p>
                  <p className="text-sm text-muted-foreground mt-1">
                    {question.text}
                  </p>
                </div>
              </div>
              
              <Textarea
                value={answers[question.id] || ''}
                onChange={(e) => {
                  setAnswers({
                    ...answers,
                    [question.id]: e.target.value,
                  });
                  if (onAnswer) {
                    onAnswer(question.id, e.target.value);
                  }
                }}
                placeholder="Sua resposta..."
                className="min-h-[80px]"
              />
            </div>
          );
        })}
        
        <Button
          onClick={() => onComplete?.(answers)}
          disabled={Object.keys(answers).length < questions.length}
          className="w-full"
        >
          Enviar Respostas
        </Button>
      </CardContent>
    </Card>
  );
}
