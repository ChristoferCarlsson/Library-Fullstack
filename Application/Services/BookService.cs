using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;

        public BookService(
            IBookRepository bookRepository,
            IAuthorRepository authorRepository,
            IMapper mapper)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _mapper = mapper;
        }

        public async Task<List<BookDto>> GetAllAsync()
        {
            var books = await _bookRepository.GetAllAsync();
            return _mapper.Map<List<BookDto>>(books);
        }

        public async Task<List<BookDto>> GetByAuthorAsync(int authorId)
        {
            var author = await _authorRepository.GetByIdAsync(authorId);
            if (author == null)
                throw new NotFoundException($"Author with id {authorId} not found.");

            var books = await _bookRepository.GetAllByAuthorAsync(authorId);
            return _mapper.Map<List<BookDto>>(books);
        }

        public async Task<BookDto> GetByIdAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
                throw new NotFoundException($"Book with id {id} not found.");

            return _mapper.Map<BookDto>(book);
        }

        public async Task<BookDto> CreateAsync(CreateBookDto dto)
        {
            var author = await _authorRepository.GetByIdAsync(dto.AuthorId);
            if (author == null)
                throw new NotFoundException($"Author with id {dto.AuthorId} not found.");

            var book = _mapper.Map<Book>(dto);

            await _bookRepository.AddAsync(book);
            await _bookRepository.SaveChangesAsync();

            return _mapper.Map<BookDto>(book);
        }

        public async Task<BookDto> UpdateAsync(int id, UpdateBookDto dto)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
                throw new NotFoundException($"Book with id {id} not found.");

            var author = await _authorRepository.GetByIdAsync(dto.AuthorId);
            if (author == null)
                throw new NotFoundException($"Author with id {dto.AuthorId} not found.");

            _mapper.Map(dto, book);

            _bookRepository.Update(book);
            await _bookRepository.SaveChangesAsync();

            return _mapper.Map<BookDto>(book);
        }

        public async Task DeleteAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
                throw new NotFoundException($"Book with id {id} not found.");

            _bookRepository.Remove(book);
            await _bookRepository.SaveChangesAsync();
        }

        public async Task<PagedResult<BookDto>> QueryAsync(BookQueryDto query)
        {
            var result = await _bookRepository.QueryAsync(query);

            return new PagedResult<BookDto>
            {
                Items = _mapper.Map<List<BookDto>>(result.Items),
                TotalCount = result.TotalCount
            };
        }
    }
}
