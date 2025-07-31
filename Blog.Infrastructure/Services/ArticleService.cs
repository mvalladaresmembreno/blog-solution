using Blog.Application.Articles;
using Blog.Application.Articles.DTOs;
using Blog.Domain.Entities;
using Blog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure.Services;

public class ArticleService : IArticleService
{
    private readonly BlogDbContext context;

    public ArticleService(BlogDbContext context)
    {
        this.context = context;
    }

    public async Task<List<ArticleDto>> GetArticlesAsync(int page, int pageSize)
    {
        var articles = await context.Articles
            .Include(a => a.Author)
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return articles.Select(a => new ArticleDto
        {
            Id = a.Id.GetHashCode(),  // Convert Guid to int key (mejor usar int Id en entidad)
            Title = a.Title,
            Summary = a.Summary,
            AuthorUsername = a.Author.Username,
            CreatedAt = a.CreatedAt
        }).ToList();
    }

    public async Task<ArticleDetailDto?> GetArticleByIdAsync(int id)
    {
        var guid = new Guid(); // Aquí deberías manejar la conversión correcta del id a Guid o usar Guid directamente
        // Si usas int en la entidad, cambia esto.

        var article = await context.Articles
            .Include(a => a.Author)
            .Include(a => a.Comments)
                .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(a => a.Id == guid);

        if (article == null) return null;

        return new ArticleDetailDto
        {
            Id = article.Id.GetHashCode(),
            Title = article.Title,
            Summary = article.Summary,
            AuthorUsername = article.Author.Username,
            CreatedAt = article.CreatedAt,
            Content = article.Content,
            Comments = article.Comments.Select(c => new CommentDto
            {
                Id = c.Id.GetHashCode(),
                Text = c.Content,
                AuthorUsername = c.Author.Username,
                CreatedAt = c.CreatedAt
            }).ToList()
        };
    }

    public async Task<ArticleDto> CreateArticleAsync(CreateArticleRequest request, string authorId)
    {
        var article = new Article
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Summary = request.Summary,
            Content = request.Content,
            AuthorId = Guid.Parse(authorId),
            CreatedAt = DateTime.UtcNow
        };

        context.Articles.Add(article);
        await context.SaveChangesAsync();

        return new ArticleDto
        {
            Id = article.Id.GetHashCode(),
            Title = article.Title,
            Summary = article.Summary,
            AuthorUsername = (await context.Users.FindAsync(article.AuthorId))?.Username ?? "unknown",
            CreatedAt = article.CreatedAt
        };
    }

    public async Task<bool> UpdateArticleAsync(int id, UpdateArticleRequest request, string userId, bool isAdmin)
    {
        var guid = new Guid(); // Ajustar conversión o usar Guid

        var article = await context.Articles.FindAsync(guid);
        if (article == null) return false;

        if (article.AuthorId != Guid.Parse(userId) && !isAdmin) return false;

        article.Title = request.Title;
        article.Summary = request.Summary;
        article.Content = request.Content;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteArticleAsync(int id, string userId, bool isAdmin)
    {
        var guid = new Guid(); // Ajustar conversión

        var article = await context.Articles.FindAsync(guid);
        if (article == null) return false;

        if (article.AuthorId != Guid.Parse(userId) && !isAdmin) return false;

        context.Articles.Remove(article);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddCommentAsync(int articleId, CreateCommentRequest request, string authorId)
    {
        var guidArticle = new Guid(); // Ajustar conversión
        var article = await context.Articles.FindAsync(guidArticle);
        if (article == null) return false;

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Content = request.Text,
            ArticleId = article.Id,
            AuthorId = Guid.Parse(authorId),
            CreatedAt = DateTime.UtcNow
        };

        context.Comments.Add(comment);
        await context.SaveChangesAsync();
        return true;
    }
}