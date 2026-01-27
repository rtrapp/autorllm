using FluentAssertions;
using AutorLLM.Application.Queries.Chapters.GetChapter;
using AutorLLM.Application.DTOs;
using MediatR;

namespace AutorLLM.Tests.Unit.Application.Queries.Chapters;

public class GetChapterQueryTests
{
    [Fact]
    public void Should_Implement_IRequest_Interface()
    {
        // Arrange & Act
        var query = new GetChapterQuery { ChapterId = Guid.NewGuid() };

        // Assert
        query.Should().BeAssignableTo<IRequest<ChapterDto>>();
    }

    [Fact]
    public void Should_Have_ChapterId_Property()
    {
        // Arrange
        var chapterId = Guid.NewGuid();

        // Act
        var query = new GetChapterQuery { ChapterId = chapterId };

        // Assert
        query.ChapterId.Should().Be(chapterId);
    }

    [Fact]
    public void Should_Be_Immutable_Record()
    {
        // Arrange
        var chapterId = Guid.NewGuid();
        var query1 = new GetChapterQuery { ChapterId = chapterId };
        var query2 = new GetChapterQuery { ChapterId = chapterId };

        // Assert
        query1.Should().Be(query2);
        query1.GetHashCode().Should().Be(query2.GetHashCode());
    }
}
