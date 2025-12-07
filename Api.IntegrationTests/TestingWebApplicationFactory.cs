using Api;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class TestingWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 1️⃣ Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<LibraryDbContext>)
            );
            if (descriptor != null)
                services.Remove(descriptor);

            // 2️⃣ Register a UNIQUE InMemory DB per test run (CI safe)
            services.AddDbContext<LibraryDbContext>(options =>
            {
                options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
            });

            // 3️⃣ Build the updated provider
            var sp = services.BuildServiceProvider();

            // 4️⃣ Seed the database
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

            db.Database.EnsureCreated();

            SeedTestData(db);
        });
    }

    private static void SeedTestData(LibraryDbContext db)
    {
        // Avoid double-seeding (only needed if running locally)
        if (db.Authors.Any())
            return;

        // Seed Authors
        db.Authors.Add(new Author
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Description = "Test Author"
        });

        // Seed Books
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

        // Seed Members
        db.Members.Add(new Member
        {
            Id = 1,
            FirstName = "Jane",
            LastName = "Tester",
            Email = "test@example.com",
            JoinedAt = DateTime.UtcNow.AddMonths(-2)
        });

        // Seed Loans
        db.Loans.Add(new Loan
        {
            Id = 1,
            BookId = 1,
            MemberId = 1,
            LoanDate = DateTime.UtcNow.AddDays(-5),
            DueDate = DateTime.UtcNow.AddDays(5),
            ReturnDate = null
        });

        db.SaveChanges();
    }
}
