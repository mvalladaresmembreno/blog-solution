namespace Blog.Domain.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public string Content { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid ArticleId { get; set; }
    public Article Article { get; set; } = default!;

    public Guid AuthorId { get; set; }
    public User Author { get; set; } = default!;
}