using System.Security.Claims;
using Blog.Application.Articles;
using Blog.Application.Articles.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticlesController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var articles = await _articleService.GetArticlesAsync(page, pageSize);
        return Ok(articles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var article = await _articleService.GetArticleByIdAsync(id);
        return article == null ? NotFound() : Ok(article);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateArticleRequest request)
    {
        var authorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var article = await _articleService.CreateArticleAsync(request, authorId!);
        return CreatedAtAction(nameof(GetById), new { id = article.Id }, article);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateArticleRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("admin");

        var success = await _articleService.UpdateArticleAsync(id, request, userId, isAdmin);
        return success ? NoContent() : Forbid();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("admin");

        var success = await _articleService.DeleteArticleAsync(id, userId, isAdmin);
        return success ? NoContent() : Forbid();
    }

    [Authorize]
    [HttpPost("{id}/comments")]
    public async Task<IActionResult> AddComment(int id, [FromBody] CreateCommentRequest request)
    {
        var authorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var success = await _articleService.AddCommentAsync(id, request, authorId);
        return success ? Ok() : NotFound();
    }
}