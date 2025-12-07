using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Services;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _bookRepo;
    private readonly Mock<IAuthorRepository> _authorRepo;
    private readonly IMapper _mapper;
    private readonly BookService _service;

    public BookServiceTests()
    {
        _bookRepo = new Mock<IBookRepository>();
        _authorRepo = new Mock<IAuthorRepository>();

        var mapperConfig = new MapperConfiguration(cfg =>
            cfg.AddProfile<Application.Mapping.LibraryProfile>());
        _mapper = mapperConfig.CreateMapper();

        _service = new BookService(
            _bookRepo.Object,
            _authorRepo.Object,
            _mapper,
            Mock.Of<ILogger<BookService>>()
        );
    }

    [Fact]
    public async Task GetById_ShouldThrow_WhenNotFound()
    {
        _bookRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Domain.Entities.Book?)null);

        var act = async () => await _service.GetByIdAsync(1);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Create_ShouldThrow_WhenAuthorMissing()
    {
        _authorRepo.Setup(r => r.GetByIdAsync(5))
                   .ReturnsAsync((Domain.Entities.Author?)null);

        var dto = new CreateBookDto
        {
            Title = "Test",
            ISBN = "TEST-ISBN",
            PublishedDate = DateTime.UtcNow,
            CopiesTotal = 1,
            AuthorId = 5
        };


        var act = async () => await _service.CreateAsync(dto);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Update_ShouldThrow_WhenBookMissing()
    {
        _bookRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Domain.Entities.Book?)null);

        var dto = new UpdateBookDto
        {
            Title = "Updated",
            ISBN = "UPDATED-ISBN",
            PublishedDate = DateTime.UtcNow,
            CopiesAvailable = 0,
            CopiesTotal = 1,
            AuthorId = 1
        };

        var act = async () => await _service.UpdateAsync(1, dto);

        await act.Should().ThrowAsync<NotFoundException>();

        _bookRepo.Verify(r => r.GetByIdAsync(1), Times.Once);
        _authorRepo.VerifyNoOtherCalls();
    }
}
