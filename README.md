FrontEnd: Razor Pages app that consumes all three microservices.

UserService: Manages user CRUD and authentication (/api/users).

AuthorService: Manages author CRUD (/api/authors).

BookService: Manages book CRUD (/api/books).

Each service has its own database context but can share a single SQL Server instance.

Setup & Run Instructions
Prerequisites
.NET 8 SDK

SQL Server (local or remote)

(Optional) Postman for API testing

(Optional) Git and a GitHub account

1. Clone the Repository
bash
git clone https://github.com/alivmran/LibraryManagement.git
cd LibraryManagement
2. Configure Connection Strings
Each service reads its connection string from appsettings.json (or appsettings.Development.json). By default, they point to localhost SQL Server. E.g. in Services/UserServices/UserServices/appsettings.json:

json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=UserDb;Trusted_Connection=True;"
},
"JwtSettings": { … }
Adjust Server= or Database= as needed for your environment.

Repeat for AuthorService and BookService.

3. Run Migrations & Seed Data (if any)
From each service folder, run:

bash
cd Services/UserServices/UserServices
dotnet ef database update

cd ../../AuthorService/AuthorService
dotnet ef database update

cd ../../BookService/BookService
dotnet ef database update
4. Run Services Locally
Open three terminals (one per service) and run:

bash
# Terminal 1
cd Services/UserServices/UserServices
dotnet run --urls "https://localhost:7175"

# Terminal 2
cd Services/AuthorService/AuthorService
dotnet run --urls "https://localhost:7183"

# Terminal 3
cd Services/BookService/BookService
dotnet run --urls "https://localhost:7265"
5. Run FrontEnd
bash
cd FrontEnd/FrontEnd
dotnet run --urls "https://localhost:5001"
Open https://localhost:5001 in your browser; the UI will call each microservice on ports 7175, 7183, 7265.
