using Entities;

namespace FileRepositories;
using RepositoryContracts;
using System.Text.Json;

public class UserFileRepository: IUserRepository
{
    private readonly string _filePath = "C:\\Users\\sefka\\RiderProjects\\DNPassigmentY\\FileRepositories\\users.json";
    private List<User> _users = new List<User>();

    public UserFileRepository()
    {
        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }
            _users = JsonSerializer.Deserialize<List<User>>(json)?? new List<User>();
        }
    }
    public Task<User> AddAsync(User user)
    {
        // Novom korisniku dodeljujemo ID.
// Prvi korisnik dobija 1, a sledeći najveći postojeći ID + 1.
        user.Id = _users.Count == 0
            ? 1
            : _users.Max(u => u.Id) + 1;
        _users.Add(user);
        var json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user)
    {
        User? existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser != null)
        {
            existingUser.UserName = user.UserName;
            existingUser.Password = user.Password;
            var json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });            File.WriteAllText(_filePath, json);
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        User? existingUser = _users.FirstOrDefault(u => u.Id == id);
        if (existingUser != null)
            {
            _users.Remove(existingUser);
        var json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
            }
        return Task.CompletedTask;
    }

    

    public Task<User> GetSingleAsync(int id)
    {
        // Tražimo korisnika sa odgovarajućim ID brojem.
        User? existingUser = _users.FirstOrDefault(u => u.Id == id);

        // Ako korisnik ne postoji, prijavljujemo grešku.
        if (existingUser == null)
        {
            throw new Exception("User not found.");
        }

        // Pošto smo proverile da korisnik postoji,
        // C# zna da existingUser više nije null.
        return Task.FromResult(existingUser);
    }


    public IQueryable<User> GetMany()
    {
        return _users.AsQueryable();
    }
}