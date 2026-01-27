using FluentAssertions;
using AutorLLM.Application.Queries.Characters.GetCharacter;
using AutorLLM.Application.DTOs;
using MediatR;

namespace AutorLLM.Tests.Unit.Application.Queries.Characters;

public class GetCharacterQueryTests
{
    [Fact]
    public void Should_Implement_IRequest_Interface()
    {
        // Arrange & Act
        var query = new GetCharacterQuery { CharacterId = Guid.NewGuid() };

        // Assert
        query.Should().BeAssignableTo<IRequest<CharacterDto>>();
    }

    [Fact]
    public void Should_Have_CharacterId_Property()
    {
        // Arrange
        var characterId = Guid.NewGuid();

        // Act
        var query = new GetCharacterQuery { CharacterId = characterId };

        // Assert
        query.CharacterId.Should().Be(characterId);
    }

    [Fact]
    public void Should_Be_Immutable_Record()
    {
        // Arrange
        var characterId = Guid.NewGuid();
        var query1 = new GetCharacterQuery { CharacterId = characterId };
        var query2 = new GetCharacterQuery { CharacterId = characterId };

        // Assert
        query1.Should().Be(query2);
        query1.GetHashCode().Should().Be(query2.GetHashCode());
    }
}
