import { BrainstormChat } from '@/components/brainstorm/BrainstormChat';
import { useNavigate } from 'react-router-dom';

export default function BrainstormPage() {
  const navigate = useNavigate();

  const handleClose = () => {
    navigate('/projects');
  };

  return (
    <div className="h-screen bg-background">
      <BrainstormChat onClose={handleClose} />
    </div>
  );
}
