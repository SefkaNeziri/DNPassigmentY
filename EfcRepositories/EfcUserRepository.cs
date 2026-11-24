using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace EfcRepositories;

// Repozitorijum za rad sa korisnicima u SQLite bazi.
public class EfcUserRepository : IUserRepository
{
    // DbContext nam omogucava pristup bazi podataka.
    private readonly ForumDbContext _context;

    // Dependency Injection prosledjuje DbContext.
    public EfcUserRepository(ForumDbContext context)
    {
        _context = context;
    }

    // CREATE: Dodajemo novog korisnika u bazu.
    public async Task<User> AddAsync(User user)
    {
        await _context.Users.AddAsync(user);

        // Cuvamo promene u bazi.
        await _context.SaveChangesAsync();

        return user;
    }

    // UPDATE: Menjamo podatke postojeceg korisnika.
    public async Task UpdateAsync(User user)
    {
        // Pronalazimo korisnika u bazi.
        User existing = await GetSingleAsync(user.Id);

        // Menjamo njegove podatke.
        existing.UserName = user.UserName;
        existing.Password = user.Password;

        // Cuvamo izmene.
        await _context.SaveChangesAsync();
    }

    // DELETE: Brisemo korisnika prema ID broju.
    public async Task DeleteAsync(int id)
    {
        User user = await GetSingleAsync(id);

        _context.Users.Remove(user);

        await _context.SaveChangesAsync();
    }

    // READ: Pronalazimo jednog korisnika prema ID broju.
    public async Task<User> GetSingleAsync(int id)
    {
        User? user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            throw new Exception("User not found.");
        }

        return user;
    }

    // READ: Vracamo upit za sve korisnike.
    public IQueryable<User> GetMany()
    {
        return _context.Users.AsQueryable();
    }
}