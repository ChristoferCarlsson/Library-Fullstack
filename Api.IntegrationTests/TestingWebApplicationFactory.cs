using Api;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class TestingWebApplicationFactory
    : WebApplicationFactory<Program>
{
    // ✅ UNIQUE DB PER FACTORY INSTANCE
    private readonly string _databaseName = $"TestDb_{Guid.NewGuid()}";

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("https_port", "0");

        builder.ConfigureServices(services =>
        {
            // Remove app DbContext
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<LibraryDbContext>)
            );
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // ✅ Unique InMemory DB
            services.AddDbContext<LibraryDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName)
            );

            // Build service provider
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

            db.Database.EnsureCreated();

            // ---------- Seed ----------
            var author = new Author
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Description = "Test author"
            };

            var member = new Member
            {
                Id = 1,
                FirstName = "Jane",
                LastName = "Tester",
                Email = "test@example.com"
            };

            var book = new Book
            {
                Id = 1,
                Title = "Seed Book",
                PublishedDate = DateTime.UtcNow.AddYears(-1),
                AuthorId = 1,
                Author = author,
                ISBN = "SEED-ISBN-0001",
                CopiesAvailable = 3,
                CopiesTotal = 3
            };

            var loan = new Loan
            {
                Id = 1,
                BookId = 1,
                Book = book,
                MemberId = 1,
                Member = member,
                LoanDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14)
            };

            db.Authors.Add(author);
            db.Members.Add(member);
            db.Books.Add(book);
            db.Loans.Add(loan);

            db.SaveChanges();
        });
    }
}
