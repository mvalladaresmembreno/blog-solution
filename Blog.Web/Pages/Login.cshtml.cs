// Pages/Auth/Login.cshtml.cs

using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Web.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LoginModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty] public string Username { get; set; } = "";

    [BindProperty] public string Password { get; set; } = "";

    [BindProperty] public string NewUsername { get; set; } = "";

    [BindProperty] public string NewPassword { get; set; } = "";

    [BindProperty] public string ConfirmPassword { get; set; } = "";

    public string? ErrorMessage { get; set; }

    public bool IsRegistering { get; set; } = false;

    public void OnGet(bool register = false)
    {
        IsRegistering = register;
    }

    public async Task<IActionResult> OnPostLoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Debe ingresar usuario y contraseña.";
            return Page();
        }

        var client = _httpClientFactory.CreateClient("API");

        var payload = new { email = Username, password = Password };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("api/auth/login", content);

        if (!response.IsSuccessStatusCode)
        {
            ErrorMessage = "Credenciales incorrectas.";
            return Page();
        }

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<LoginResponse>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Response.Cookies.Append("jwt", data!.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, Username) // o lo que venga del token
        };

        var identity = new ClaimsIdentity(claims, "MyCookieAuth");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("MyCookieAuth", principal);

        return RedirectToPage("/Articles/Index");
    }

    public async Task<IActionResult> OnPostRegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(NewUsername) || string.IsNullOrWhiteSpace(NewPassword) ||
            string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            ErrorMessage = "Todos los campos son obligatorios.";
            IsRegistering = true;
            return Page();
        }

        if (NewPassword != ConfirmPassword)
        {
            ErrorMessage = "Las contraseñas no coinciden.";
            IsRegistering = true;
            return Page();
        }

        var client = _httpClientFactory.CreateClient("API");

        var payload = new { Username = NewUsername, Email = NewUsername, Password = NewPassword };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("api/auth/register", content);

        if (response.IsSuccessStatusCode) return RedirectToPage("/Login");

        ErrorMessage = "Error al registrar usuario.";
        IsRegistering = true;
        return Page();

        // Registro exitoso, vuelve al login
    }

    private class LoginResponse
    {
        public string Token { get; set; } = "";
    }
}