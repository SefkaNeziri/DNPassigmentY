using Entities;
using Microsoft.EntityFrameworkCore;

namespace EfcRepositories;

// DbContext predstavlja vezu izmedju C# aplikacije i baze podataka.
public class ForumDbContext : DbContext
{
    // Svaki DbSet predstavlja tabelu u bazi podataka.
    public DbSet<User> Users => Set<User>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Comment> Comments => Set<Comment>();

    // Konstruktor prima konfiguraciju za bazu.
    public ForumDbContext(
        DbContextOptions<ForumDbContext> options) : base(options)
    {
    }

    // Ovde definisemo odnose izmedju entiteta.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Korisnicko ime je obavezno.
        modelBuilder.Entity<User>()
            .Property(u => u.UserName)
            .IsRequired();

        // Jedan korisnik moze imati vise objava.
        modelBuilder.Entity<Post>()
            .HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Jedan korisnik moze napisati vise komentara.
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Jedna objava moze imati vise komentara.
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}