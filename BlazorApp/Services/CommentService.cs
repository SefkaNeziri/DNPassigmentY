using System.Net.Http.Json;
using ApiContracts;

namespace BlazorApp.Services;

// Servis za komunikaciju sa API-jem za komentare.
public class CommentService : ICommentService
{
    private readonly HttpClient _httpClient;

    public CommentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Preuzimamo samo komentare određene objave.
    public async Task<List<CommentDto>> GetCommentsAsync(int postId)
    {
        List<CommentDto>? comments =
            await _httpClient.GetFromJsonAsync<List<CommentDto>>(
                $"api/comments?postId={postId}");

        return comments ?? new List<CommentDto>();
    }

    // Kreiramo novi komentar.
    public async Task<CommentDto> AddCommentAsync(
        CreateCommentDto request)
    {
        HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync("api/comments", request);

        response.EnsureSuccessStatusCode();

        CommentDto? comment =
            await response.Content.ReadFromJsonAsync<CommentDto>();

        return comment ?? throw new Exception("Could not create comment.");
    }
}