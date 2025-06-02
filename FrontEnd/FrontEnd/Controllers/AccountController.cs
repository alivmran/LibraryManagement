using Library.Frontend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library.Frontend.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _factory;
        private readonly string _baseUrl;

        public AccountController(IHttpClientFactory f, IConfiguration cfg)
        {
            _factory = f;
            _baseUrl = cfg["ServiceUrls:UserServices"]!.TrimEnd('/');
        }

        // GET /Account/Login
        public IActionResult Login() => View();

        // POST /Account/Login
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel m)
        {
            if (!ModelState.IsValid)
                return View(m);

            var client = _factory.CreateClient("UserServices");
            var resp = await client.PostAsJsonAsync(
                            $"{_baseUrl}/api/users/login", m);

            if (!resp.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View(m);
            }

            var json = await resp.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            var token = json!["token"];

            // Store in session
            HttpContext.Session.SetString("JWToken", token);

            return RedirectToAction("Index", "Home");
        }

        // GET /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWToken");
            return RedirectToAction("Login");
        }
    }
}
