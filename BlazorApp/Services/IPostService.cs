using ApiContracts;

namespace BlazorApp.Services;

// Interfejs za rad sa objavama.
public interface IPostService
{
    Task<List<PostDto>> GetPostsAsync();
    Task<PostDto> GetPostAsync(int id);
    Task<PostDto> AddPostAsync(CreatePostDto request);
}
