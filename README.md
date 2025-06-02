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

API Documentation
UserService (https://localhost:7175/api/users)
Endpoint	Method	Description
/api/users	GET	Get all users
/api/users/{id}	GET	Get a single user by ID
/api/users	POST	Create a new user
/api/users/{id}	PUT	Update existing user
/api/users/{id}	DELETE	Delete user by ID
/api/users/login	POST	Authenticate user (returns JWT)

Sample: Create User
http
POST /api/users
Content-Type: application/json

{
  "username": "jdoe",
  "email": "jdoe@example.com",
  "password": "P@ssw0rd"
}
AuthorService (https://localhost:7183/api/authors)
Endpoint	Method	Description
/api/authors	GET	Get all authors
/api/authors/{id}	GET	Get author by ID
/api/authors	POST	Create a new author
/api/authors/{id}	DELETE	Delete author by ID

Sample: Create Author
http
POST /api/authors
Content-Type: application/json

{
  "name": "Jane Austen"
}
BookService (https://localhost:7265/api/books)
Endpoint	Method	Description
/api/books	GET	Get all books
/api/books/{id}	GET	Get book by ID
/api/books	POST	Create a new book
/api/books/{id}	PUT	Update existing book
/api/books/{id}	DELETE	Delete book by ID

Sample: Create Book
http
POST /api/books
Content-Type: application/json

{
  "title": "Pride and Prejudice",
  "isbn": "9780141199078",
  "authorId": 1
}
