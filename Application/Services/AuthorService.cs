using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthorService> _logger;

        public AuthorService(
            IAuthorRepository authorRepository,
            IMapper mapper,
            ILogger<AuthorService> logger)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<AuthorDto>> GetAllAsync()
        {
            var authors = await _authorRepository.GetAllAsync();
            _logger.LogInformation("Fetched {Count} authors", authors.Count);

            return _mapper.Map<List<AuthorDto>>(authors);
        }

        public async Task<AuthorDto> GetByIdAsync(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id);

            if (author == null)
            {
                _logger.LogWarning("Author with ID {Id} not found", id);
                throw new NotFoundException($"Author with id {id} not found.");
            }

            _logger.LogInformation("Fetched author with ID {Id}", id);
            return _mapper.Map<AuthorDto>(author);
        }

        public async Task<AuthorDto> CreateAsync(CreateAuthorDto dto)
        {
            var author = _mapper.Map<Author>(dto);

            await _authorRepository.AddAsync(author);
            await _authorRepository.SaveChangesAsync();

            _logger.LogInformation("Created author with ID {Id}", author.Id);

            return _mapper.Map<AuthorDto>(author);
        }

        public async Task<AuthorDto> UpdateAsync(int id, UpdateAuthorDto dto)
        {
            var author = await _authorRepository.GetByIdAsync(id);

            if (author == null)
            {
                _logger.LogWarning("Attempted update: Author with ID {Id} not found", id);
                throw new NotFoundException($"Author with id {id} not found.");
            }

            _mapper.Map(dto, author);

            _authorRepository.Update(author);
            await _authorRepository.SaveChangesAsync();

            _logger.LogInformation("Updated author with ID {Id}", id);

            return _mapper.Map<AuthorDto>(author);
        }

        public async Task DeleteAsync(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id);

            if (author == null)
            {
                _logger.LogWarning("Attempted delete: Author with ID {Id} not found", id);
                throw new NotFoundException($"Author with id {id} not found.");
            }

            var books = await _authorRepository.GetBooksByAuthorIdAsync(id);

            if (books.Any())
            {
                _logger.LogWarning(
                    "Cannot delete author {Id}: They still have {Count} books",
                    id, books.Count
                );

                throw new ValidationException(
                    "Cannot delete author because they still have books assigned."
                );
            }

            _authorRepository.Remove(author);
            await _authorRepository.SaveChangesAsync();

            _logger.LogWarning("Deleted author with ID {Id}", id);
        }
    }
}
