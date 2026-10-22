namespace ApiContracts;

// Podaci koje šaljemo kada kreiramo novu objavu.
public class CreatePostDto
{
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public int UserId { get; set; }
}
