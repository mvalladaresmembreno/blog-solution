using Blog.Application.Articles.DTOs;

namespace Blog.Application.Articles;

public interface IArticleService
{
    Task<List<ArticleDto>> GetArticlesAsync(int page, int pageSize);
    Task<ArticleDetailDto?> GetArticleByIdAsync(int id);
    Task<ArticleDto> CreateArticleAsync(CreateArticleRequest request, string authorId);
    Task<bool> UpdateArticleAsync(int id, UpdateArticleRequest request, string userId, bool isAdmin);
    Task<bool> DeleteArticleAsync(int id, string userId, bool isAdmin);
    Task<bool> AddCommentAsync(int articleId, CreateCommentRequest request, string authorId);
}