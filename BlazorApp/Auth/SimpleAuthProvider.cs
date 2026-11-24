
using System.Net.Http.Json;
using System.Security.Claims;
using ApiContracts;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorApp.Auth;

// Ova klasa cuva informacije o prijavljenom korisniku.
public class SimpleAuthProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;

    // Na pocetku nijedan korisnik nije prijavljen.
    private ClaimsPrincipal _currentUser = new(
        new ClaimsIdentity());

    // HttpClient dobijamo preko Dependency Injection-a.
    public SimpleAuthProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Saljemo korisnicko ime i lozinku Web API-ju.
    public async Task LoginAsync(string userName, string password)
    {
        LoginRequest request = new()
        {
            UserName = userName,
            Password = password
        };

        HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync("auth/login", request);

        // Ako podaci nisu tacni, prijavljivanje nije uspelo.
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Invalid username or password.");
        }

        // Preuzimamo korisnika bez lozinke.
        UserDto? user =
            await response.Content.ReadFromJsonAsync<UserDto>();

        if (user == null)
        {
            throw new Exception("Login failed.");
        }

        // Claims sadrze informacije o prijavljenom korisniku.
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        // Pravimo identitet prijavljenog korisnika.
        ClaimsIdentity identity = new(claims, "ForumLogin");

        _currentUser = new ClaimsPrincipal(identity);

        // Obavestavamo Blazor da se korisnik prijavio.
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(_currentUser)));
    }

    // Blazor ovom metodom proverava ko je prijavljen.
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(_currentUser));
    }

    // Odjavljujemo korisnika.
    public void Logout()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        // Obavestavamo Blazor da se korisnik odjavio.
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(_currentUser)));
    }
}

