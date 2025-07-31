namespace Blog.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string Role { get; set; } = "user"; // "admin" o "user"

    public ICollection<Article> Articles { get; set; } = new List<Article>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}