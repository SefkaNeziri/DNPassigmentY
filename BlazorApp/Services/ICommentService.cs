using ApiContracts;

namespace BlazorApp.Services;

// Interfejs za rad sa komentarima.
public interface ICommentService
{
    Task<List<CommentDto>> GetCommentsAsync(int postId);
    Task<CommentDto> AddCommentAsync(CreateCommentDto request);
}