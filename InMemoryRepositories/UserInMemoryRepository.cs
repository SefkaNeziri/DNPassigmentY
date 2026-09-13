using Entities;
using RepositoryContracts;
namespace InMemoryRepositories;

public class UserInMemoryRepository :IUserRepository
{
    private readonly List<User> users = new List<User>();

    public Task<User> AddAsync(User user)
    {
        int newId = 1;

        if (users.Count > 0)
        {
            newId = users.Count + 1;
        }

        user.Id = newId;
        users.Add(user);

        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user)
    {
        for (int i = 0; i < users.Count; i++)
        {
            if (users[i].Id == user.Id)
            {
                users[i] = user;
                return Task.CompletedTask;
            }
        }

        throw new Exception("User was not found.");
    }

    public Task DeleteAsync(int id)
    {
        for (int i = 0; i < users.Count; i++)
        {
            if (users[i].Id == id)
            {
                users.RemoveAt(i);
                return Task.CompletedTask;
            }
        }

        throw new Exception("User was not found.");
    }

    public Task<User> GetSingleAsync(int id)
    {
        foreach (User user in users)
        {
            if (user.Id == id)
            {
                return Task.FromResult(user);
            }
        }

        throw new Exception("User was not found.");
    }

    public IQueryable<User> GetMany()
    {
        return users.AsQueryable();
    }
}