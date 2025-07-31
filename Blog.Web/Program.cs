using Blog.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
var configuration = builder.Configuration;
var apiBaseUrl = configuration["ApiSettings:BaseUrl"];

// Esto ya lo tienes, necesario para acceder a HttpContext
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<JwtAuthorizationHandler>();

builder.Services.AddHttpClient("API", client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl!);
    })
    .AddHttpMessageHandler<JwtAuthorizationHandler>();


// Agrega autenticación por cookies:
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Login";  // Página a la que redirige si no está autenticado
        options.Cookie.Name = "myapp-auth";   // Puedes poner otro nombre si quieres
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ¡Agrega esto en este orden!
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages()
    .WithStaticAssets();

app.Run();