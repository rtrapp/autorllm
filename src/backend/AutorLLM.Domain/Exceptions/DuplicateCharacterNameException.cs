namespace AutorLLM.Domain.Exceptions;

public class DuplicateCharacterNameException : DomainException
{
    public string CharacterName { get; }

    public DuplicateCharacterNameException(string characterName) 
        : base($"A character with the name '{characterName}' already exists in this project.")
    {
        CharacterName = characterName;
    }
}
