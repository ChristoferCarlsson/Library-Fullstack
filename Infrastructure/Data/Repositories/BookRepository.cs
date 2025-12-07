using Application.DTOs;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repositories
{
    public class BookRepository : IBookRepository
    {

        private readonly LibraryDbContext _context;

        public BookRepository(LibraryDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Book book)
        {
            await _context.Books.AddAsync(book);
        }

        public async Task<List<Book>> GetAllAsync()
        {
            return await _context.Books
                .Include(b => b.Author)
                .ToListAsync();
        }

        public async Task<List<Book>> GetAllByAuthorAsync(int authorId)
        {
            return await _context.Books
                .Where(b => b.AuthorId == authorId)
                .Include(b => b.Author)
                .ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books
                .FirstOrDefaultAsync(b => b.Id == id);
        }
        public void Remove(Book book)
        {
            _context.Books.Remove(book);
        }
        public void Update(Book book)
        {
            _context.Books.Update(book);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagedResult<Book>> QueryAsync(BookQueryDto query)
        {
            var books = _context.Books
                .Include(b => b.Author)
                .AsQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(query.Title))
                books = books.Where(b => b.Title.Contains(query.Title));

            if (query.AuthorId.HasValue)
                books = books.Where(b => b.AuthorId == query.AuthorId);

            if (!string.IsNullOrWhiteSpace(query.ISBN))
                books = books.Where(b => b.ISBN == query.ISBN);

            // Sorting
            books = query.SortBy switch
            {
                "PublishedDate" =>
                    query.Desc ? books.OrderByDescending(b => b.PublishedDate) :
                                 books.OrderBy(b => b.PublishedDate),

                "CopiesAvailable" =>
                    query.Desc ? books.OrderByDescending(b => b.CopiesAvailable) :
                                 books.OrderBy(b => b.CopiesAvailable),

                _ =>
                    query.Desc ? books.OrderByDescending(b => b.Title) :
                                 books.OrderBy(b => b.Title),
            };

            // Pagination
            var total = await books.CountAsync();

            var items = await books
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PagedResult<Book>
            {
                Items = items,
                TotalCount = total
            };
        }

    }
}
