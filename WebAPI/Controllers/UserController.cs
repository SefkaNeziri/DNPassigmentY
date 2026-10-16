
using ApiContracts; // Naše DTO klase.
using Entities; // Naša klasa User.
using Microsoft.AspNetCore.Mvc; // Omogućava HTTP zahteve i odgovore.
using RepositoryContracts; // Interfejs IUserRepository.

namespace WebAPI.Controllers; // Namespace za Web API kontrolere.

// Govori ASP.NET-u da je ova klasa API kontroler.
[ApiController]

// Osnovna adresa kontrolera je /api/users.
[Route("api/users")]
public class UsersController : ControllerBase
{
    // Repozitorijum za pristup korisnicima.
    private readonly IUserRepository _userRepository;

    // Dependency Injection automatski prosleđuje repozitorijum.
    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // GET /api/users
    // Prikazuje korisnike i omogućava pretragu po imenu.
    [HttpGet]
    public IActionResult GetMany([FromQuery] string? username)
    {
        // Uzimamo sve korisnike iz repozitorijuma.
        IQueryable<User> users = _userRepository.GetMany();

        // Ako je unet tekst za pretragu, filtriramo korisnike.
        if (!string.IsNullOrWhiteSpace(username))
        {
            users = users.Where(u =>
                u.UserName.Contains(
                    username,
                    StringComparison.OrdinalIgnoreCase));
        }

        // Pretvaramo User objekte u UserDto.
        // Ne prenosimo lozinke!
        List<UserDto> result = users.Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.UserName
        }).ToList();

        // Vraćamo HTTP 200 OK sa listom korisnika.
        return Ok(result);
    }

    // GET /api/users/1
    // Prikazuje jednog korisnika prema ID broju.
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSingle(int id)
    {
        try
        {
            // Tražimo korisnika u repozitorijumu.
            User user = await _userRepository.GetSingleAsync(id);

            // Pravimo DTO koji ne sadrži lozinku.
            UserDto dto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName
            };

            // Vraćamo pronađenog korisnika.
            return Ok(dto);
        }
        catch (Exception)
        {
            // Ako korisnik ne postoji, vraćamo HTTP 404.
            return NotFound("User not found.");
        }
    }

    // POST /api/users
    // Kreira novog korisnika.
    [HttpPost]
    public async Task<IActionResult> Add(
        [FromBody] CreateUserDto request)
    {
        // Od podataka iz DTO-a pravimo User objekat.
        User user = new User
        {
            UserName = request.UserName,
            Password = request.Password
        };

        // Čuvamo korisnika i dobijamo njegov ID.
        User created = await _userRepository.AddAsync(user);

        // Pravimo odgovor bez lozinke.
        UserDto dto = new UserDto
        {
            Id = created.Id,
            UserName = created.UserName
        };

        // HTTP 201 znači da je korisnik kreiran.
        // Navodimo i adresu na kojoj može da se pronađe.
        return CreatedAtAction(
            nameof(GetSingle),
            new { id = dto.Id },
            dto);
    }

    // PUT /api/users/1
    // Menja korisničko ime i lozinku.
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] CreateUserDto request)
    {
        try
        {
            // Proveravamo da li korisnik sa tim ID brojem postoji.
            await _userRepository.GetSingleAsync(id);

            // Pravimo User objekat sa novim podacima.
            User updatedUser = new User
            {
                Id = id,
                UserName = request.UserName,
                Password = request.Password
            };

            // Čuvamo izmenjene podatke.
            await _userRepository.UpdateAsync(updatedUser);

            // HTTP 204 znači da je izmena uspela.
            return NoContent();
        }
        catch (Exception)
        {
            // Ako korisnik ne postoji, vraćamo 404.
            return NotFound("User not found.");
        }
    }

    // DELETE /api/users/1
    // Briše korisnika prema ID broju.
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            // Pokušavamo da pronađemo korisnika.
            await _userRepository.GetSingleAsync(id);

            // Brišemo korisnika iz repozitorijuma.
            await _userRepository.DeleteAsync(id);

            // HTTP 204 znači uspešno brisanje.
            return NoContent();
        }
        catch (Exception)
        {
            // Ako korisnik ne postoji, vraćamo 404.
            return NotFound("User not found.");
        }
    }
}
