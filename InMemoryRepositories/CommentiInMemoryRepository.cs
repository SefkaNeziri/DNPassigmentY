using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class CommentInMemoryRepository :ICommentRepository
{
    private readonly List<Comment> comments = new List<Comment>();

    public Task<Comment> AddAsync(Comment comment)
    {
        int newId = 1;

        if (comments.Count > 0)
        {
            newId = comments.Count + 1;
        }

        comment.Id = newId;
        comments.Add(comment);

        return Task.FromResult(comment);
    }

    public Task UpdateAsync(Comment comment)
    {
        for (int i = 0; i < comments.Count; i++)
        {
            if (comments[i].Id == comment.Id)
            {
                comments[i] = comment;
                return Task.CompletedTask;
            }
        }

        throw new Exception("Comment was not found.");
    }

    public Task DeleteAsync(int id)
    {
        for (int i = 0; i < comments.Count; i++)
        {
            if (comments[i].Id == id)
            {
                comments.RemoveAt(i);
                return Task.CompletedTask;
            }
        }

        throw new Exception("Comment was not found.");
    }

    public Task<Comment> GetSingleAsync(int id)
    {
        foreach (Comment comment in comments)
        {
            if (comment.Id == id)
            {
                return Task.FromResult(comment);
            }
        }

        throw new Exception("Comment was not found.");
    }

    public IQueryable<Comment> GetMany()
    {
        return comments.AsQueryable();
    }
}