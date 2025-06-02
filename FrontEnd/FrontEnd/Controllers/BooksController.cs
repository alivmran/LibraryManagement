using Library.Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;

namespace Library.Frontend.Controllers
{
    public class BooksController : Controller
    {
        private readonly IHttpClientFactory _factory;
        private readonly string _baseUrl;

        public BooksController(IHttpClientFactory factory, IConfiguration cfg)
        {
            _factory = factory;
            _baseUrl = cfg["ServiceUrls:BookService"]!.TrimEnd('/');
        }

        private HttpClient CreateClient()
        {
            var client = _factory.CreateClient("BookService");
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        // GET: /Books
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("JWToken") is null)
                return RedirectToAction("Login", "Account");

            List<BookViewModel> books;
            try
            {
                var client = CreateClient();
                var resp = await client.GetAsync($"{_baseUrl}/api/books");
                resp.EnsureSuccessStatusCode();
                books = await resp.Content.ReadFromJsonAsync<List<BookViewModel>>()
                        ?? new List<BookViewModel>();
            }
            catch
            {
                books = new List<BookViewModel>();
            }
            return View(books);
        }

        // GET: /Books/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("JWToken") is null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        // POST: /Books/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookViewModel m)
        {
            if (HttpContext.Session.GetString("JWToken") is null)
                return RedirectToAction("Login", "Account");
            if (!ModelState.IsValid) return View(m);

            var client = CreateClient();
            await client.PostAsJsonAsync($"{_baseUrl}/api/books", new
            {
                Title = m.Title,
                ISBN = m.ISBN,
                AuthorId = m.AuthorId
            });
            return RedirectToAction(nameof(Index));
        }

        // GET: /Books/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("JWToken") is null)
                return RedirectToAction("Login", "Account");

            var client = CreateClient();
            var book = await client.GetFromJsonAsync<BookViewModel>(
                $"{_baseUrl}/api/books/{id}");
            return book is null ? NotFound() : View(book);
        }

        // POST: /Books/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookViewModel m)
        {
            if (HttpContext.Session.GetString("JWToken") is null)
                return RedirectToAction("Login", "Account");
            if (id != m.Id || !ModelState.IsValid) return View(m);

            var client = CreateClient();
            await client.PutAsJsonAsync($"{_baseUrl}/api/books/{id}", m);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Books/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("JWToken") is null)
                return RedirectToAction("Login", "Account");

            var client = CreateClient();
            var book = await client.GetFromJsonAsync<BookViewModel>(
                $"{_baseUrl}/api/books/{id}");
            return book is null ? NotFound() : View(book);
        }

        // POST: /Books/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("JWToken") is null)
                return RedirectToAction("Login", "Account");

            var client = CreateClient();
            await client.DeleteAsync($"{_baseUrl}/api/books/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
