using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Tp2Api.Model;

public partial class Tp2Context : DbContext
{
    public Tp2Context()
    {
    }

    public Tp2Context(DbContextOptions<Tp2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Knowledge> Knowledges { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Database=tp2;Username=postgres;Password=admin")
            .UseSnakeCaseNamingConvention();
    
    [DbFunction("insert_users", "api")]
    public bool InsertUsers(string usernameapp, string passwordapp, string usernamepg, string passwordpg)
        => throw new NotSupportedException();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDbFunction(typeof(Tp2Context).GetMethod(nameof(InsertUsers), new[] { typeof(string), typeof(string), typeof(string), typeof(string) }))
            .HasName("insert_users").HasSchema("api");
        
        modelBuilder.Entity<Knowledge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("knowledges_pkey");

            entity.ToTable("knowledges");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Data)
                .HasColumnType("jsonb")
                .HasColumnName("data");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users", "api");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Isadmin).HasColumnName("isadmin");
            entity.Property(e => e.Lastactivity)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("lastactivity");
            entity.Property(e => e.Passwordapp).HasColumnName("passwordapp");
            entity.Property(e => e.Passwordpg).HasColumnName("passwordpg");
            entity.Property(e => e.Usernameapp).HasColumnName("usernameapp");
            entity.Property(e => e.Usernamepg).HasColumnName("usernamepg");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
