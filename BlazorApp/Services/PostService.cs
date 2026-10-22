using System.Net.Http.Json;
using ApiContracts;

namespace BlazorApp.Services;

// Servis za komunikaciju sa API-jem za objave.
public class PostService : IPostService
{
    private readonly HttpClient _httpClient;

    public PostService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Preuzimamo sve objave.
    public async Task<List<PostDto>> GetPostsAsync()
    {
        List<PostDto>? posts =
            await _httpClient.GetFromJsonAsync<List<PostDto>>("api/posts");

        return posts ?? new List<PostDto>();
    }

    // Preuzimamo objavu prema njenom ID-ju.
    public async Task<PostDto> GetPostAsync(int id)
    {
        PostDto? post =
            await _httpClient.GetFromJsonAsync<PostDto>($"api/posts/{id}");

        return post ?? throw new Exception("Post not found.");
    }

    // Šaljemo novu objavu Web API-ju.
    public async Task<PostDto> AddPostAsync(CreatePostDto request)
    {
        HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync("api/posts", request);

        response.EnsureSuccessStatusCode();

        PostDto? post =
            await response.Content.ReadFromJsonAsync<PostDto>();

        return post ?? throw new Exception("Could not create post.");
    }
}