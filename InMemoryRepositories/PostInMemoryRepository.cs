using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class PostInMemoryRepository  : IPostRepository
{
    private readonly List<Post> posts = new List<Post>();

    public Task<Post> AddAsync(Post post)
    {
        int newId = 1;

        if (posts.Count > 0)
        {
            newId = posts.Count + 1;
        }

        post.Id = newId;
        posts.Add(post);

        return Task.FromResult(post);
    }

    public Task UpdateAsync(Post post)
    {
        for (int i = 0; i < posts.Count; i++)
        {
            if (posts[i].Id == post.Id)
            {
                posts[i] = post;
                return Task.CompletedTask;
            }
        }

        throw new Exception("Post was not found.");
    }

    public Task DeleteAsync(int id)
    {
        for (int i = 0; i < posts.Count; i++)
        {
            if (posts[i].Id == id)
            {
                posts.RemoveAt(i);
                return Task.CompletedTask;
            }
        }

        throw new Exception("Post was not found.");
    }

    public Task<Post> GetSingleAsync(int id)
    {
        foreach (Post post in posts)
        {
            if (post.Id == id)
            {
                return Task.FromResult(post);
            }
        }

        throw new Exception("Post was not found.");
    }

    public IQueryable<Post> GetMany()
    {
        return posts.AsQueryable();
    }
}