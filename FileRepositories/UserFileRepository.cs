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
        _users.Add(user);
        var json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetSingleAsync(int id)
    {
        throw new NotImplementedException();
    }

    public IQueryable<User> GetMany()
    {
        throw new NotImplementedException();
    }
}