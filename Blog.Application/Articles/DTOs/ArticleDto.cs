namespace Blog.Application.Articles.DTOs;
public class ArticleDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Summary { get; set; } = null!;
    public string AuthorUsername { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

public class ArticleDetailDto : ArticleDto
{
    public string Content { get; set; } = null!;
    public List<CommentDto> Comments { get; set; } = new();
}

public class CommentDto
{
    public int Id { get; set; }
    public string Text { get; set; } = null!;
    public string AuthorUsername { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

public class CreateArticleRequest
{
    public string Title { get; set; } = null!;
    public string Summary { get; set; } = null!;
    public string Content { get; set; } = null!;
}

public class UpdateArticleRequest : CreateArticleRequest { }

public class CreateCommentRequest
{
    public string Text { get; set; } = null!;
}