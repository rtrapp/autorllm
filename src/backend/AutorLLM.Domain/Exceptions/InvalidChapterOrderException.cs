namespace AutorLLM.Domain.Exceptions;

public class InvalidChapterOrderException : DomainException
{
    public int Order { get; }

    public InvalidChapterOrderException(int order) 
        : base($"Invalid chapter order: {order}. Order must be greater than 0.")
    {
        Order = order;
    }
}
