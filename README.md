📚 Library Management System – Fullstack Application
Kunskapskontroll 1 – Individuell Inlämning

Kurs: Objektorienterad Programmering – Avancerad

📖 Översikt

Detta projekt är en komplett fullstack-applikation byggd för att hantera ett bibliotekssystem med:

Bokhantering (CRUD + sökning + filtrering + pagination)
Författarhantering (CRUD + skydd mot borttagning av författare som har böcker)
Medlemshantering (CRUD)
Utlåning och återlämning av böcker (inklusive lagerlogik)

Dashboard med statistik
Applikationen är byggd enligt modern branschpraxis, med tydlig lagerarkitektur, testbar kod, CI-pipeline och en React-frontend.

🏗 Tekniska verktyg
Lager	Teknik
Backend	.NET 8 Web API, EF Core, SQL Server
Frontend	React (Vite), Material UI
Database	Azure SQL Database
Testing	xUnit, Moq, Real EF Integration Tests
CI/CD	GitHub Actions (restore → build → test)
Arkitektur	Clean-ish Architecture (Domain, Application, Infrastructure, API)
🧩 Arkitektur
📦 Projektstruktur
Library/
│
├── Api/                    → Controllers, Startup, Middleware
├── Application/            → Services, Interfaces, DTOs, Mapping
├── Domain/                 → Entities (Book, Author, Member, Loan)
├── Infrastructure/         → EF Core, Repositories, DbContext
│
├── Application.Tests/      → Unit tests for service layer
└── Api.IntegrationTests/   → Integration tests for controllers

🏛 Arkitekturdiagram
┌─────────────────────────┐
│        Presentation      │   → React Frontend
└─────────────┬───────────┘
              │ REST API calls
┌─────────────▼───────────┐
│        API Layer         │   → Controllers, Validation, Error Middleware
└─────────────┬───────────┘
              │ Calls services
┌─────────────▼───────────┐
│    Application Layer     │   → Services, DTOs, Mapping, Business Logic
└─────────────┬───────────┘
              │ Uses repositories
┌─────────────▼───────────┐
│   Infrastructure Layer   │   → EF Core, Repositories, SQL
└─────────────┬───────────┘
              │
┌─────────────▼───────────┐
│     Domain Layer         │   → Entities only
└──────────────────────────┘

🔧 Backend – Installation & Körning
1️⃣ Klona projektet
git clone <project-url>
cd Library

2️⃣ Konfigurera appsettings

Backend använder Azure SQL Database.
i Api manage user secrets.

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:<server>.database.windows.net;Database=<db>;User Id=<user>;Password=<password>;Encrypt=True;"
  }
}

3️⃣ Kör API
cd Api
dotnet restore
dotnet run

API startar normalt på:
https://localhost:7068

🎨 Frontend – Installation & Körning
1️⃣ Installera paket
cd frontend
npm install

2️⃣ Starta utvecklingsservern
npm run dev

Frontend körs normalt på:
http://localhost:5173

🔌 API Endpoints
📚 Books
Method	Route	Description
GET	/api/books	Get all books
GET	/api/books/{id}	Get book by id
GET	/api/books/search	Search, filter, paginate
POST	/api/books	Create book
PUT	/api/books/{id}	Update book
DELETE	/api/books/{id}	Delete book
✍️ Authors
Method	Route	Description
GET	/api/authors	Get all authors
GET	/api/authors/{id}	Get author by id
POST	/api/authors	Create
PUT	/api/authors/{id}	Update
DELETE	/api/authors/{id}
👤 Members
Method	Route	Description
GET	/api/members	Get all members
POST	/api/members	Create
PUT	/api/members/{id}	Update
DELETE	/api/members/{id}	Delete
📘 Loans
Method	Route	Description
GET	/api/loans	Get all loans
POST	/api/loans	Create new loan
PUT	/api/loans/{id}	Update loan
PUT	/api/loans/{id}/return	Return a book
📊 Dashboard Features

Dashboard hämtas via:

GET /api/dashboard


Ger statistik:

Totalt antal böcker
Totalt antal medlemmar
Aktiva lån
Försenade lån
Frontend visar detta överst på sidan.

🧪 Testning
✔ Unit tests (Application.Tests)

Täcker:

BookService
AuthorService
MemberService
LoanService
DashboardService

Exempel på scenarier:

Skapa
Uppdatera
Felhantering (NotFound, ValidationException)
Kopieringslogik vid lån och returnering

✔ Integration tests (Api.IntegrationTests)

Använder TestingWebApplicationFactory.
Täcker controllers:
BooksController
AuthorsController
MembersController
LoansController
DashboardController

Testar:

HTTP-statuskoder
Response bodies
Felhantering via middleware
Databasinteraktion (in-memory EF)

⚙️ CI/CD – GitHub Actions

Workflow:
name: CI
on: [push, pull_request]
jobs:
  build-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 8.0.x

      - name: Restore
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore

      - name: Test
        run: dotnet test --no-build --verbosity normal


Pipeline körs automatiskt vid:

push
pull request
