
using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace EfcRepositories;

// Repozitorijum za rad sa objavama u SQLite bazi.
public class EfcPostRepository : IPostRepository
{
    // DbContext nam omogucava pristup bazi podataka.
    private readonly ForumDbContext _context;

    // Konstruktor dobija DbContext preko Dependency Injection-a.
    public EfcPostRepository(ForumDbContext context)
    {
        _context = context;
    }

    // CREATE: Dodajemo novu objavu.
    public async Task<Post> AddAsync(Post post)
    {
        // Dodajemo objavu u DbContext.
        await _context.Posts.AddAsync(post);

        // Cuvamo novu objavu u SQLite bazi.
        await _context.SaveChangesAsync();

        // Vracamo objavu sa dodeljenim ID-jem.
        return post;
    }

    // UPDATE: Menjamo postojecu objavu.
    public async Task UpdateAsync(Post post)
    {
        // Pronalazimo objavu koju zelimo da izmenimo.
        Post existing = await GetSingleAsync(post.Id);

        // Menjamo njene podatke.
        existing.Title = post.Title;
        existing.Body = post.Body;
        existing.UserId = post.UserId;

        // Cuvamo promene u bazi.
        await _context.SaveChangesAsync();
    }

    // DELETE: Brisemo objavu prema ID-ju.
    public async Task DeleteAsync(int id)
    {
        // Pronalazimo objavu.
        Post post = await GetSingleAsync(id);

        // Oznacavamo je za brisanje.
        _context.Posts.Remove(post);

        // Izvrsavamo brisanje u bazi.
        await _context.SaveChangesAsync();
    }

    // READ: Pronalazimo jednu objavu prema ID-ju.
    public async Task<Post> GetSingleAsync(int id)
    {
        Post? post = await _context.Posts
            .FirstOrDefaultAsync(p => p.Id == id);

        // Ako objava ne postoji, prijavljujemo gresku.
        if (post == null)
        {
            throw new Exception("Post not found.");
        }

        return post;
    }

    // READ: Vracamo upit za sve objave.
    public IQueryable<Post> GetMany()
    {
        return _context.Posts.AsQueryable();
    }
}
