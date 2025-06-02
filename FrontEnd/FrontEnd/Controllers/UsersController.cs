using Library.Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;

namespace Library.Frontend.Controllers
{
    public class UsersController : Controller
    {
        private readonly IHttpClientFactory _factory;
        private readonly string _baseUrl;

        public UsersController(IHttpClientFactory factory, IConfiguration cfg)
        {
            _factory = factory;
            _baseUrl = cfg["ServiceUrls:UserServices"]!.TrimEnd('/');
        }

        // Attach the JWT from session to the HttpClient
        private HttpClient CreateClient()
        {
            var client = _factory.CreateClient("UserServices");
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }

        // GET: /Users
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("JWToken") == null)
                return RedirectToAction("Login", "Account");

            List<UserViewModel> users;
            try
            {
                var client = CreateClient();
                var resp = await client.GetAsync($"{_baseUrl}/api/users");
                resp.EnsureSuccessStatusCode();
                users = await resp.Content
                                .ReadFromJsonAsync<List<UserViewModel>>()
                         ?? new List<UserViewModel>();
            }
            catch
            {
                users = new List<UserViewModel>();
            }

            return View(users);
        }

        // GET: /Users/Register
        public IActionResult Register() => View();

        // POST: /Users/Register
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel m)
        {
            if (!ModelState.IsValid)
                return View(m);

            var client = CreateClient();
            await client.PostAsJsonAsync($"{_baseUrl}/api/users", new
            {
                Username = m.Username,
                Email = m.Email,
                PasswordHash = m.Password
            });

            return RedirectToAction("Login", "Account");
        }

        // GET: /Users/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (HttpContext.Session.GetString("JWToken") == null)
                return RedirectToAction("Login", "Account");

            var client = CreateClient();
            var user = await client.GetFromJsonAsync<UserViewModel>($"{_baseUrl}/api/users/{id}");
            return user is null ? NotFound() : View(user);
        }

        // GET: /Users/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("JWToken") == null)
                return RedirectToAction("Login", "Account");

            var client = CreateClient();
            var user = await client.GetFromJsonAsync<UserViewModel>($"{_baseUrl}/api/users/{id}");
            return user is null ? NotFound() : View(user);
        }

        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("JWToken") == null)
                return RedirectToAction("Login", "Account");

            var client = CreateClient();
            await client.DeleteAsync($"{_baseUrl}/api/users/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
