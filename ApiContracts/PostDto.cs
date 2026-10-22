namespace ApiContracts;

// Podaci o objavi koje dobijamo od Web API-ja.
public class PostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public int UserId { get; set; }
}