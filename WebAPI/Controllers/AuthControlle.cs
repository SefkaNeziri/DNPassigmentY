using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

// Oznacava klasu kao Web API kontroler.
[ApiController]

// Osnovna adresa za prijavljivanje korisnika.
[Route("auth")]
public class AuthController : ControllerBase
{
    // Repozitorijum koristimo za pronalazenje korisnika.
    private readonly IUserRepository _userRepository;

    // Konstruktor prima repozitorijum preko Dependency Injection-a.
    public AuthController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // POST zahtev na adresu /auth/login.
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Pronalazimo korisnika prema korisnickom imenu.
        User? user = _userRepository.GetMany()
            .FirstOrDefault(u => u.UserName == request.UserName);

        // Proveravamo da li korisnik postoji i da li je lozinka tacna.
        if (user == null || user.Password != request.Password)
        {
            // HTTP 401 znaci da prijavljivanje nije uspelo.
            return Unauthorized("Invalid username or password.");
        }

        // Kreiramo DTO koji ne sadrzi korisnikovu lozinku.
        UserDto result = new()
        {
            Id = user.Id,
            UserName = user.UserName
        };

        // HTTP 200 OK vraca podatke prijavljenog korisnika.
        return Ok(result);
    }
}