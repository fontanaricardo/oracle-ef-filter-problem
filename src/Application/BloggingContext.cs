using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Application.Generators;

namespace Application;

public class BloggingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }

    public string DbPath { get; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseOracle(@"connectionstring");
        optionsBuilder.ReplaceService<IMigrationsSqlGenerator, CustomCSharpMigrationsGenerator>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Blog>()
            .HasIndex(e => e.Url )
            .IsUnique()
            .HasFilter("\"IsDeleted\" = 0");
    }
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }
    public int IsDeleted { get; set; } = default;

    public List<Post> Posts { get; } = new();
}

public class Post
{
    public int PostId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }

    public int BlogId { get; set; }
    public Blog Blog { get; set; }
}