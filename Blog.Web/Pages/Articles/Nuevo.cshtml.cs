using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Web.Pages.Articles;

public class NuevoModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    [BindProperty]
    public Articulo Articulo { get; set; } = new();

    public NuevoModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public void OnGet()
    {
        Articulo.FechaPublicacion = DateTime.Now;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var client = _httpClientFactory.CreateClient("API");
        var jwt = Request.Cookies["jwt"];
        if (string.IsNullOrEmpty(jwt))
            return RedirectToPage("/Login");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        var content = new StringContent(JsonSerializer.Serialize(Articulo), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/articles", content);

        if (response.IsSuccessStatusCode)
            return RedirectToPage("Index");

        ModelState.AddModelError(string.Empty, "Error al crear el artículo.");
        return Page();
    }
}