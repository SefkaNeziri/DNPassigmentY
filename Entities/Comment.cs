using System.Text.Json.Serialization;

namespace Entities;

// Predstavlja jedan komentar na forumu.
public class Comment
{
    // Jedinstveni ID komentara.
    public int Id { get; set; }

    // Tekst komentara.
    public string Body { get; set; } = "";

    // ID korisnika koji je napisao komentar.
    public int UserId { get; set; }

    // ID objave kojoj komentar pripada.
    public int PostId { get; set; }

    // Autor komentara.
    [JsonIgnore]
    public User User { get; set; } = null!;

    // Objava na kojoj je komentar napisan.
    [JsonIgnore]
    public Post Post { get; set; } = null!;
}