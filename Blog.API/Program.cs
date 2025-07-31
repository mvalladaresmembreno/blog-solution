using System.Text;
using Blog.Application.Articles;
using Blog.Application.Auth;
using Blog.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Blog.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios OpenAPI (Swagger)
builder.Services.AddOpenApi();

// Agregar capa de infraestructura y EF Core (SQLite)
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlite("Data Source=blog.db"));
// Registrar AuthService
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IArticleService, ArticleService>();

// Configurar JWT desde appsettings.json
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"[AUTH ERROR] {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine($"[AUTH CHALLENGE] {context.Error}, {context.ErrorDescription}");
                return Task.CompletedTask;
            }
        };
    });
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAuthorization();

// Agregar controladores y Razor Pages
builder.Services.AddControllers();
builder.Services.AddRazorPages();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configurar pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.Run();