namespace Blog.Domain.Entities;

public class Article
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Summary { get; set; } = default!;
    public string Content { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid AuthorId { get; set; }
    public User Author { get; set; } = default!;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}