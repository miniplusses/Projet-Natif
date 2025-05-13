using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ORM.Models; 

public class MonContexte : DbContext {

    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Post { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseNpgsql("Host=localhost;Database=testORM;Username=postgres;Password=admin")
            .UseSnakeCaseNamingConvention();
}

public class Blog {
    public int Id { get; set; }
    public string Url { get; set; }
    
    public List<Post> Posts { get; set; }
}

public class Post {
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    
    public int BlogId { get; set; }
    public Blog Blog { get; set; }
}