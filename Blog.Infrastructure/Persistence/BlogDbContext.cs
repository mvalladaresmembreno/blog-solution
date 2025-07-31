using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure.Persistence;

public class BlogDbContext(DbContextOptions<BlogDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Article>()
            .HasOne(a => a.Author)
            .WithMany()
            .HasForeignKey(a => a.AuthorId);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Author)
            .WithMany()
            .HasForeignKey(c => c.AuthorId);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Article)
            .WithMany(a => a.Comments)
            .HasForeignKey(c => c.ArticleId);
    }
}