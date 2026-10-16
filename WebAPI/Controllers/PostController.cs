
using Entities; // Naša klasa Post.
using RepositoryContracts; // Interfejs IPostRepository.
using Microsoft.AspNetCore.Mvc; // HTTP metode i odgovori.

namespace WebAPI.Controllers; // Prostor imena za kontrolere.

// Govori ASP.NET-u da je ovo API kontroler.
[ApiController]

// Osnovna adresa za objave: /api/posts.
[Route("api/posts")]
public class PostsController : ControllerBase
{
    // Repozitorijum za pristup objavama.
    private readonly IPostRepository _postRepository;

    // Konstruktor dobija repozitorijum kroz Dependency Injection.
    public PostsController(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    // GET /api/posts
    // Vraća sve objave.
    // Opcionalno filtrira prema naslovu ili ID-ju autora.
    [HttpGet]
    public IActionResult GetMany(
        [FromQuery] string? title,
        [FromQuery] int? userId)
    {
        // Uzimamo sve objave kao upit.
        IQueryable<Post> posts = _postRepository.GetMany();

        // Ako je korisnik uneo deo naslova, filtriramo.
        if (!string.IsNullOrWhiteSpace(title))
        {
            posts = posts.Where(p => p.Title.Contains(title));
        }

        // Ako je prosleđen userId, prikazujemo samo objave tog autora.
        if (userId.HasValue)
        {
            posts = posts.Where(p => p.UserId == userId.Value);
        }

        // Izvršavamo upit i vraćamo HTTP 200 OK.
        return Ok(posts.ToList());
    }

    // GET /api/posts/1
    // Vraća jednu objavu prema ID-ju.
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSingle(int id)
    {
        try
        {
            // Tražimo objavu u našem repozitorijumu.
            Post post = await _postRepository.GetSingleAsync(id);

            // Ako postoji, vraćamo HTTP 200 OK.
            return Ok(post);
        }
        catch (Exception)
        {
            // Ako objava ne postoji, vraćamo HTTP 404.
            return NotFound("Post not found.");
        }
    }

    // POST /api/posts
    // Dodaje novu objavu.
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] Post post)
    {
        // Repozitorijum čuva objavu i dodeljuje joj ID.
        Post created = await _postRepository.AddAsync(post);

        // HTTP 201 Created znači da je objava napravljena.
        // Klijentu vraćamo adresu nove objave.
        return CreatedAtAction(
            nameof(GetSingle),
            new { id = created.Id },
            created);
    }

    // PUT /api/posts/1
    // Menja postojeću objavu.
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] Post post)
    {
        // Proveravamo da se ID iz URL-a i tela zahteva podudaraju.
        if (id != post.Id)
        {
            return BadRequest("Post ID does not match.");
        }

        try
        {
            // Menjamo objavu u repozitorijumu.
            await _postRepository.UpdateAsync(post);

            // HTTP 204 znači da je izmena uspela.
            return NoContent();
        }
        catch (Exception)
        {
            // Ako objava ne postoji, vraćamo 404.
            return NotFound("Post not found.");
        }
    }

    // DELETE /api/posts/1
    // Briše objavu prema ID-ju.
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            // Brišemo objavu iz repozitorijuma.
            await _postRepository.DeleteAsync(id);

            // HTTP 204 znači da je brisanje uspelo.
            return NoContent();
        }
        catch (Exception)
        {
            // Ako objava ne postoji, vraćamo 404.
            return NotFound("Post not found.");
        }
    }
}
