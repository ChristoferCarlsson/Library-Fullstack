using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Services;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

public class AuthorServiceTests
{
    private readonly Mock<IAuthorRepository> _repoMock;
    private readonly AuthorService _service;
    private readonly IMapper _mapper;

    public AuthorServiceTests()
    {
        _repoMock = new Mock<IAuthorRepository>();

        var mapperConfig = new MapperConfiguration(cfg =>
            cfg.AddProfile<Application.Mapping.LibraryProfile>());
        _mapper = mapperConfig.CreateMapper();

        _service = new AuthorService(
            _repoMock.Object,
            _mapper,
            Mock.Of<ILogger<AuthorService>>()
        );
    }

    [Fact]
    public async Task GetById_ShouldReturnAuthor_WhenExists()
    {
        var author = new Domain.Entities.Author { Id = 1, FirstName = "John", LastName = "Wick" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(author);

        var result = await _service.GetByIdAsync(1);

        result.Id.Should().Be(1);
        result.FullName.Should().Be("John Wick");
    }

    [Fact]
    public async Task GetById_ShouldThrow_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Domain.Entities.Author?)null);

        var act = async () => await _service.GetByIdAsync(1);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Create_ShouldAddAuthor()
    {
        var dto = new CreateAuthorDto { FirstName = "Jane", LastName = "Doe" };

        _repoMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Author>()))
                 .Returns(Task.CompletedTask);

        _repoMock.Setup(r => r.SaveChangesAsync())
                 .ReturnsAsync(true);

        var result = await _service.CreateAsync(dto);

        result.FullName.Should().Be("Jane Doe");

        _repoMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Author>()), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Update_ShouldThrow_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Domain.Entities.Author?)null);

        var dto = new UpdateAuthorDto { FirstName = "Test", LastName = "User" };

        var act = async () => await _service.UpdateAsync(1, dto);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Delete_ShouldRemoveAuthor()
    {
        var author = new Domain.Entities.Author { Id = 1 };

        _repoMock.Setup(r => r.GetByIdAsync(1))
                 .ReturnsAsync(author);

        _repoMock.Setup(r => r.GetBooksByAuthorIdAsync(1))
                 .ReturnsAsync(new List<Domain.Entities.Book>());

        await _service.DeleteAsync(1);

        _repoMock.Verify(r => r.Remove(author), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

}
