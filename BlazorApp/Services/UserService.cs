using System.Net.Http.Json;
using ApiContracts;

namespace BlazorApp.Services;

// Servis za slanje HTTP zahteva koji se odnose na korisnike.
public class UserService : IUserService
{
    private readonly HttpClient _httpClient;

    // HttpClient dobijamo preko Dependency Injection-a.
    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Preuzimamo sve korisnike.
    public async Task<List<UserDto>> GetUsersAsync()
    {
        List<UserDto>? users =
            await _httpClient.GetFromJsonAsync<List<UserDto>>("api/users");

        return users ?? new List<UserDto>();
    }

    // Preuzimamo jednog korisnika prema njegovom ID-ju.
    public async Task<UserDto> GetUserAsync(int id)
    {
        UserDto? user =
            await _httpClient.GetFromJsonAsync<UserDto>($"api/users/{id}");

        return user ?? throw new Exception("User not found.");
    }

    // Kreiramo novog korisnika.
    public async Task<UserDto> AddUserAsync(CreateUserDto request)
    {
        HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync("api/users", request);

        response.EnsureSuccessStatusCode();

        UserDto? user =
            await response.Content.ReadFromJsonAsync<UserDto>();

        return user ?? throw new Exception("Could not create user.");
    }
}