namespace ApiContracts;

// Podaci koje korisnik unosi prilikom prijavljivanja.
public class LoginRequest
{
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
}