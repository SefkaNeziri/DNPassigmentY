using System.Text.Json.Serialization;

namespace Entities;

// Predstavlja jednog korisnika foruma.
public class User
{
    // Jedinstveni ID korisnika.
    public int Id { get; set; }

    // Korisnicko ime.
    public string UserName { get; set; } = "";

    // Lozinka korisnika.
    public string Password { get; set; } = "";

    // Jedan korisnik moze napisati vise objava.
    [JsonIgnore]
    public List<Post> Posts { get; set; } = new();

    // Jedan korisnik moze napisati vise komentara.
    [JsonIgnore]
    public List<Comment> Comments { get; set; } = new();
}