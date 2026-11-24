using System.Text.Json.Serialization;

namespace Entities;

// Predstavlja jednu objavu na forumu.
public class Post
{
    // Jedinstveni ID objave.
    public int Id { get; set; }

    // Naslov objave.
    public string Title { get; set; } = "";

    // Sadrzaj objave.
    public string Body { get; set; } = "";

    // ID korisnika koji je napisao objavu.
    public int UserId { get; set; }

    // Korisnik koji je autor ove objave.
    [JsonIgnore]
    public User User { get; set; } = null!;

    // Svi komentari koji pripadaju ovoj objavi.
    [JsonIgnore]
    public List<Comment> Comments { get; set; } = new();
}
