using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Mapping;
using Application.Services;
using AutoMapper;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Api.Middleware;

Console.WriteLine(" PROGRAM LOADED FROM API PROJECT ");

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------------
// CORS CONFIGURATION (Frontend runs on http://localhost:5173)
// --------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LibraryDatabase"))
);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// AutoMapper
builder.Services.AddAutoMapper(typeof(LibraryProfile));

// Repositories
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();

// Services
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Use HTTPS only when NOT testing
if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}

// --------------------------------------------------------
// ENABLE CORS (must be BEFORE Authorization & MapControllers)
// --------------------------------------------------------
app.UseCors("AllowFrontend");

app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
