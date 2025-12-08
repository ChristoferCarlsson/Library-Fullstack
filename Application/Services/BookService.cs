using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BookService> _logger;

        public BookService(
            IBookRepository bookRepository,
            IAuthorRepository authorRepository,
            IMapper mapper,
            ILogger<BookService> logger)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<BookDto>> GetAllAsync()
        {
            var books = await _bookRepository.GetAllAsync();
            _logger.LogInformation("Fetched {Count} books", books.Count);
            return _mapper.Map<List<BookDto>>(books);
        }

        public async Task<List<BookDto>> GetByAuthorAsync(int authorId)
        {
            var author = await _authorRepository.GetByIdAsync(authorId);
            if (author == null)
            {
                _logger.LogWarning("Attempted to fetch books for non-existent author ID {AuthorId}", authorId);
                throw new NotFoundException($"Author with id {authorId} not found.");
            }

            var books = await _bookRepository.GetAllByAuthorAsync(authorId);
            _logger.LogInformation("Fetched {Count} books for author ID {AuthorId}", books.Count, authorId);

            return _mapper.Map<List<BookDto>>(books);
        }

        public async Task<BookDto> GetByIdAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                _logger.LogWarning("Book with ID {Id} not found", id);
                throw new NotFoundException($"Book with id {id} not found.");
            }

            _logger.LogInformation("Fetched book with ID {Id}", id);
            return _mapper.Map<BookDto>(book);
        }

        public async Task<BookDto> CreateAsync(CreateBookDto dto)
        {
            var author = await _authorRepository.GetByIdAsync(dto.AuthorId);
            if (author == null)
            {
                _logger.LogWarning("Attempted to create book with non-existent author ID {AuthorId}", dto.AuthorId);
                throw new NotFoundException($"Author with id {dto.AuthorId} not found.");
            }

            var book = _mapper.Map<Book>(dto);

            book.CopiesAvailable = dto.CopiesTotal;

            await _bookRepository.AddAsync(book);
            await _bookRepository.SaveChangesAsync();

            _logger.LogInformation("Created book with ID {Id}", book.Id);

            return _mapper.Map<BookDto>(book);
        }

        public async Task<BookDto> UpdateAsync(int id, UpdateBookDto dto)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                _logger.LogWarning("Attempted update: Book with ID {Id} not found", id);
                throw new NotFoundException($"Book with id {id} not found.");
            }

            var author = await _authorRepository.GetByIdAsync(dto.AuthorId);
            if (author == null)
            {
                _logger.LogWarning("Attempted book update with invalid Author ID {AuthorId}", dto.AuthorId);
                throw new NotFoundException($"Author with id {dto.AuthorId} not found.");
            }

            _mapper.Map(dto, book);

            if (book.CopiesAvailable > dto.CopiesTotal)
            {
                book.CopiesAvailable = dto.CopiesTotal;
            }

            book.CopiesTotal = dto.CopiesTotal;

            _bookRepository.Update(book);
            await _bookRepository.SaveChangesAsync();

            _logger.LogInformation("Updated book with ID {Id}", id);

            return _mapper.Map<BookDto>(book);
        }

        public async Task DeleteAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                _logger.LogWarning("Attempted delete: Book with ID {Id} not found", id);
                throw new NotFoundException($"Book with id {id} not found.");
            }

            _bookRepository.Remove(book);
            await _bookRepository.SaveChangesAsync();

            _logger.LogWarning("Deleted book with ID {Id}", id);
        }

        public async Task<PagedResult<BookDto>> QueryAsync(BookQueryDto query)
        {
            var result = await _bookRepository.QueryAsync(query);

            _logger.LogInformation(
                "Queried books: {Count} results (Page {Page}, PageSize {PageSize})",
                result.Items.Count,
                query.Page,
                query.PageSize
            );

            return new PagedResult<BookDto>
            {
                Items = _mapper.Map<List<BookDto>>(result.Items),
                TotalCount = result.TotalCount
            };
        }
    }
}
