
using Entities; // Omogućava korišćenje klase Comment.
using RepositoryContracts; // Interfejs ICommentRepository.
using Microsoft.AspNetCore.Mvc; // HTTP zahtevi i odgovori.

namespace WebAPI.Controllers; // Namespace naših kontrolera.

// Govori ASP.NET-u da je ova klasa API kontroler.
[ApiController]

// Osnovna adresa kontrolera je /api/comments.
[Route("api/comments")]
public class CommentsController : ControllerBase
{
    // Repozitorijum koji pristupa sačuvanim komentarima.
    private readonly ICommentRepository _commentRepository;

    // Dependency Injection nam prosleđuje repozitorijum.
    public CommentsController(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    // GET /api/comments
    // Prikazuje sve komentare ili ih filtrira po userId i postId.
    [HttpGet]
    public IActionResult GetMany(
        [FromQuery] int? userId,
        [FromQuery] int? postId)
    {
        // Preuzimamo komentare kao IQueryable.
        IQueryable<Comment> comments = _commentRepository.GetMany();

        // Ako je prosleđen userId, uzimamo komentare tog korisnika.
        if (userId.HasValue)
        {
            comments = comments.Where(c => c.UserId == userId.Value);
        }

        // Ako je prosleđen postId, uzimamo komentare te objave.
        if (postId.HasValue)
        {
            comments = comments.Where(c => c.PostId == postId.Value);
        }

        // ToList izvršava upit i pravi listu rezultata.
        // Ok vraća HTTP status 200.
        return Ok(comments.ToList());
    }

    // GET /api/comments/1
    // Pronalazi jedan komentar prema ID broju.
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSingle(int id)
    {
        try
        {
            // Tražimo komentar u repozitorijumu.
            Comment comment = await _commentRepository.GetSingleAsync(id);

            // Vraćamo pronađeni komentar.
            return Ok(comment);
        }
        catch (Exception)
        {
            // Ako ne postoji, vraćamo HTTP 404.
            return NotFound("Comment not found.");
        }
    }

    // POST /api/comments
    // Kreira novi komentar.
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] Comment comment)
    {
        // Čuvamo komentar i dobijamo njegov novi ID.
        Comment created = await _commentRepository.AddAsync(comment);

        // HTTP 201 Created označava uspešno kreiranje.
        return CreatedAtAction(
            nameof(GetSingle),
            new { id = created.Id },
            created);
    }

    // PUT /api/comments/1
    // Menja postojeći komentar.
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] Comment comment)
    {
        // Proveravamo da se ID iz URL-a poklapa sa ID-jem komentara.
        if (id != comment.Id)
        {
            return BadRequest("Comment ID does not match.");
        }

        try
        {
            // Čuvamo izmenjeni komentar.
            await _commentRepository.UpdateAsync(comment);

            // HTTP 204 znači da je izmena uspešna.
            return NoContent();
        }
        catch (Exception)
        {
            // Ako komentar ne postoji, vraćamo 404.
            return NotFound("Comment not found.");
        }
    }

    // DELETE /api/comments/1
    // Briše komentar prema ID broju.
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            // Brišemo komentar iz repozitorijuma.
            await _commentRepository.DeleteAsync(id);

            // HTTP 204 znači da je komentar obrisan.
            return NoContent();
        }
        catch (Exception)
        {
            // Ako komentar ne postoji, vraćamo 404.
            return NotFound("Comment not found.");
        }
    }
}
