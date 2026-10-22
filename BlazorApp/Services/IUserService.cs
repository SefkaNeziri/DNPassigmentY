using ApiContracts;

namespace BlazorApp.Services;

// Interfejs definiše šta servis za korisnike može da radi.
public interface IUserService
{
    Task<List<UserDto>> GetUsersAsync();
    Task<UserDto> GetUserAsync(int id);
    Task<UserDto> AddUserAsync(CreateUserDto request);
}