namespace ApiContracts;

// Podaci koje šaljemo kada dodajemo komentar.
public class CreateCommentDto
{
    public string Body { get; set; } = "";
    public int UserId { get; set; }
    public int PostId { get; set; }
}