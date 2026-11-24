
using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace EfcRepositories;

// Repozitorijum za rad sa komentarima u SQLite bazi.
public class EfcCommentRepository : ICommentRepository
{
    // DbContext koristimo za pristup bazi podataka.
    private readonly ForumDbContext _context;

    // Konstruktor dobija DbContext preko Dependency Injection-a.
    public EfcCommentRepository(ForumDbContext context)
    {
        _context = context;
    }

    // CREATE: Dodajemo novi komentar.
    public async Task<Comment> AddAsync(Comment comment)
    {
        // Dodajemo komentar u DbContext.
        await _context.Comments.AddAsync(comment);

        // Cuvamo komentar u SQLite bazi.
        await _context.SaveChangesAsync();

        // Vracamo komentar sa dodeljenim ID-jem.
        return comment;
    }

    // UPDATE: Menjamo postojeci komentar.
    public async Task UpdateAsync(Comment comment)
    {
        // Pronalazimo komentar koji zelimo da izmenimo.
        Comment existing = await GetSingleAsync(comment.Id);

        // Menjamo njegove podatke.
        existing.Body = comment.Body;
        existing.UserId = comment.UserId;
        existing.PostId = comment.PostId;

        // Cuvamo promene u bazi.
        await _context.SaveChangesAsync();
    }

    // DELETE: Brisemo komentar prema ID-ju.
    public async Task DeleteAsync(int id)
    {
        // Pronalazimo komentar.
        Comment comment = await GetSingleAsync(id);

        // Oznacavamo ga za brisanje.
        _context.Comments.Remove(comment);

        // Izvrsavamo brisanje u bazi.
        await _context.SaveChangesAsync();
    }

    // READ: Pronalazimo jedan komentar prema ID-ju.
    public async Task<Comment> GetSingleAsync(int id)
    {
        Comment? comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == id);

        // Ako komentar ne postoji, prijavljujemo gresku.
        if (comment == null)
        {
            throw new Exception("Comment not found.");
        }

        return comment;
    }

    // READ: Vracamo upit za sve komentare.
    public IQueryable<Comment> GetMany()
    {
        return _context.Comments.AsQueryable();
    }
}
