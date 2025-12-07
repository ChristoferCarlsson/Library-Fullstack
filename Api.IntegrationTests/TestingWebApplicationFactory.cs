using Api;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

public class TestingWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove SQL Server DbContext
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<LibraryDbContext>)
            );
            if (descriptor != null)
                services.Remove(descriptor);

            // Add InMemory DbContext
            services.AddDbContext<LibraryDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Seed test data
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
            db.Database.EnsureCreated();

            // ⭐⭐⭐ SEED DATA START ⭐⭐⭐

            // Only seed if empty (prevents duplicates)
            if (!db.Authors.Any())
            {
                db.Authors.Add(new Author
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Description = "Test Author"
                });
            }

            if (!db.Books.Any())
            {
                db.Books.Add(new Book
                {
                    Id = 1,
                    Title = "Test Book",
                    PublishedDate = DateTime.UtcNow.AddYears(-1),
                    AuthorId = 1,
                    ISBN = "TEST-ISBN-001",
                    CopiesAvailable = 3,
                    CopiesTotal = 3
                });
            }

            if (!db.Members.Any())
            {
                db.Members.Add(new Member
                {
                    Id = 1,
                    FirstName = "Jane",
                    LastName = "Tester",
                    Email = "test@example.com",
                    JoinedAt = DateTime.UtcNow.AddMonths(-2)
                });
            }

            if (!db.Loans.Any())
            {
                db.Loans.Add(new Loan
                {
                    Id = 1,
                    BookId = 1,
                    MemberId = 1,
                    LoanDate = DateTime.UtcNow.AddDays(-5),
                    DueDate = DateTime.UtcNow.AddDays(5),
                    ReturnDate = null
                });
            }

            db.SaveChanges();

            // ⭐⭐⭐ SEED DATA END ⭐⭐⭐
        });
    }
}
