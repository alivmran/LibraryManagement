using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// 1. MVC controllers + views
builder.Services.AddControllersWithViews();

// 2. Session support (must be before builder.Build())
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 3. Named HttpClients for your microservices
var svc = builder.Configuration.GetSection("ServiceUrls");
builder.Services.AddHttpClient("UserServices", client =>
{
    client.BaseAddress = new Uri(svc["UserServices"]!);
    client.DefaultRequestHeaders.Accept.Add(
      new MediaTypeWithQualityHeaderValue("application/json"));
});
builder.Services.AddHttpClient("AuthorService", client =>
{
    client.BaseAddress = new Uri(svc["AuthorService"]!);
    client.DefaultRequestHeaders.Accept.Add(
      new MediaTypeWithQualityHeaderValue("application/json"));
});
builder.Services.AddHttpClient("BookService", client =>
{
    client.BaseAddress = new Uri(svc["BookService"]!);
    client.DefaultRequestHeaders.Accept.Add(
      new MediaTypeWithQualityHeaderValue("application/json"));
});

var app = builder.Build();

// 4. Static files
app.UseStaticFiles();

// 5. Session middleware
app.UseSession();

// 6. Routing
app.UseRouting();

// 7. (Optional) Authentication/Authorization if you added them
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
