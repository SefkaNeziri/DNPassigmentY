namespace ApiContracts; // Prostor imena za DTO klase.

// Podaci koje klijent šalje kada pravi novog korisnika.
public class CreateUserDto
{
    // Ime novog korisnika.
    public string UserName { get; set; } = "";

    // Lozinka novog korisnika.
    public string Password { get; set; } = "";

    // ID ne unosimo jer ga dodeljuje repository.
}