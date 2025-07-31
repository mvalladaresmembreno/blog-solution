using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Web.Pages.Articles;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<IndexModel> _logger;

    public List<Articulo> Articulos { get; set; } = [];

    public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync("/api/articles");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            Articulos = JsonSerializer.Deserialize<List<Articulo>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
        }
        else
        {
            _logger.LogError("Error al obtener artículos: {StatusCode}", response.StatusCode);
        }
    }
}

public class Articulo
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    public string Titulo { get; set; }

    [Required(ErrorMessage = "El autor es obligatorio")]
    public string Autor { get; set; }

    [Required(ErrorMessage = "El contenido es obligatorio")]
    public string Contenido { get; set; }

    [Required(ErrorMessage = "La fecha de publicación es obligatoria")]
    public DateTime FechaPublicacion { get; set; }
}