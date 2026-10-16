namespace ApiContracts; // Prostor imena za DTO klase.

// Podaci o korisniku koje API sme da pošalje klijentu.
public class UserDto
{
    // Jedinstveni broj korisnika.
    public int Id { get; set; }

    // Korisničko ime.
    public string UserName { get; set; } = "";

    // Ovde nema Password, jer ne želimo da šaljemo lozinku.
}