using Library.Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;

namespace Library.Frontend.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IHttpClientFactory _factory;
        private readonly string _baseUrl;

        public AuthorsController(IHttpClientFactory factory, IConfiguration cfg)
        {
            _factory = factory;
            _baseUrl = cfg["ServiceUrls:AuthorService"]!.TrimEnd('/');
        }

        // Helper to get a client with JWT attached
        private HttpClient CreateClient()
        {
            var client = _factory.CreateClient("AuthorService");
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        // GET: /Authors
        public async Task<IActionResult> Index()
        {
            // Redirect to login if not authenticated
            if (HttpContext.Session.GetString("JWToken") is null)
                return RedirectToAction("Login", "Account");

            List<AuthorViewModel> authors;
            try
            {
                var client = CreateClient();
                authors = await client
                    .GetFromJsonAsync<List<AuthorViewModel>>($"{_baseUrl}/api/authors")
                    ?? new List<AuthorViewModel>();
            }
            catch
            {
                authors = new List<AuthorViewModel>();
            }
            return View(authors);
        }

        // GET: /Authors/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("JWToken") is null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        // POST: /Authors/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuthorViewModel m)
        {
            if (HttpContext.Session.GetString("JWToken") is null)
                return RedirectToAction("Login", "Account");
            if (!ModelState.IsValid) return View(m);

            var client = CreateClient();
            await client.PostAsJsonAsync($"{_baseUrl}/api/authors", m);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Authors/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("JWToken") is null)
                return RedirectToAction("Login", "Account");

            var client = CreateClient();
            var author = await client.GetFromJsonAsync<AuthorViewModel>(
                $"{_baseUrl}/api/authors/{id}");
            return author is null ? NotFound() : View(author);
        }

        // POST: /Authors/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AuthorViewModel m)
        {
            if (HttpContext.Session.GetString("JWToken") is null)
                return RedirectToAction("Login", "Account");
            if (id != m.Id || !ModelState.IsValid) return View(m);

            var client = CreateClient();
            await client.PutAsJsonAsync($"{_baseUrl}/api/authors/{id}", m);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Authors/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("JWToken") is null)
                return RedirectToAction("Login", "Account");

            var client = CreateClient();
            var author = await client.GetFromJsonAsync<AuthorViewModel>(
                $"{_baseUrl}/api/authors/{id}");
            return author is null ? NotFound() : View(author);
        }

        // POST: /Authors/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("JWToken") is null)
                return RedirectToAction("Login", "Account");

            var client = CreateClient();
            await client.DeleteAsync($"{_baseUrl}/api/authors/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
