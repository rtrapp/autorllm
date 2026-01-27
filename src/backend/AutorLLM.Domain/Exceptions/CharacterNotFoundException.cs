namespace AutorLLM.Domain.Exceptions;

public class CharacterNotFoundException : DomainException
{
    public Guid CharacterId { get; }

    public CharacterNotFoundException(Guid characterId) 
        : base($"Character with ID '{characterId}' was not found.")
    {
        CharacterId = characterId;
    }
}
