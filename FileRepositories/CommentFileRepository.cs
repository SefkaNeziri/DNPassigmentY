
using System.Text.Json; // Omogućava rad sa JSON formatom.
using Entities; // Omogućava korišćenje klase Comment.
using RepositoryContracts; // Omogućava korišćenje ICommentRepository.

namespace FileRepositories; // Namespace za naše file repository klase.

public class CommentFileRepository : ICommentRepository
{
    // Lista komentara koje smo učitali ili napravili.
    private List<Comment> _comments = new List<Comment>();

    // Određujemo gde će se nalaziti comments.json.
    private readonly string _filePath =
        Path.Combine(AppContext.BaseDirectory, "comments.json");

    // Konstruktor učitava komentare kada napravimo repository.
    public CommentFileRepository()
    {
        // Proveravamo da li fajl već postoji.
        if (File.Exists(_filePath))
        {
            // Čitamo tekst iz comments.json.
            string json = File.ReadAllText(_filePath);

            // Ako je fajl prazan, prekidamo učitavanje.
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            // Pretvaramo JSON tekst u listu komentara.
            // Ako dobijemo null, uzimamo praznu listu.
            _comments = JsonSerializer.Deserialize<List<Comment>>(json)
                        ?? new List<Comment>();
        }
    }

    // Privatna pomoćna metoda za čuvanje komentara.
    private void Save()
    {
        // Pretvaramo sve komentare u čitljiv JSON tekst.
        string json = JsonSerializer.Serialize(
            _comments,
            new JsonSerializerOptions { WriteIndented = true });

        // Upisujemo tekst u comments.json.
        File.WriteAllText(_filePath, json);
    }

    // Dodajemo novi komentar.
    public Task<Comment> AddAsync(Comment comment)
    {
        // Dodeljujemo novi, jedinstveni ID.
        // Ako je lista prazna, ID je 1.
        // Inače je za jedan veći od najvećeg postojećeg.
        comment.Id = _comments.Count == 0
            ? 1
            : _comments.Max(c => c.Id) + 1;

        // Dodajemo komentar u listu.
        _comments.Add(comment);

        // Čuvamo sve komentare u comments.json.
        Save();

        // Vraćamo dodati komentar.
        return Task.FromResult(comment);
    }

    // Menjamo postojeći komentar.
    public Task UpdateAsync(Comment comment)
    {
        // Pronalazimo komentar sa istim ID brojem.
        Comment? existingComment =
            _comments.FirstOrDefault(c => c.Id == comment.Id);

        // Ako komentar nije pronađen, prijavljujemo grešku.
        if (existingComment == null)
        {
            throw new Exception("Comment not found.");
        }

        // Menjamo tekst komentara.
        existingComment.Body = comment.Body;

        // Menjamo ID korisnika koji je napisao komentar.
        existingComment.UserId = comment.UserId;

        // Menjamo ID objave kojoj komentar pripada.
        existingComment.PostId = comment.PostId;

        // Čuvamo promene u fajlu.
        Save();

        // Vraćamo završen Task.
        return Task.CompletedTask;
    }

    // Brišemo komentar na osnovu njegovog ID-a.
    public Task DeleteAsync(int id)
    {
        // Tražimo komentar sa datim ID brojem.
        Comment? existingComment =
            _comments.FirstOrDefault(c => c.Id == id);

        // Ako ga nema, prijavljujemo grešku.
        if (existingComment == null)
        {
            throw new Exception("Comment not found.");
        }

        // Uklanjamo komentar iz liste.
        _comments.Remove(existingComment);

        // Čuvamo ažuriranu listu.
        Save();

        // Vraćamo završen Task.
        return Task.CompletedTask;
    }

    // Vraćamo jedan komentar na osnovu njegovog ID-a.
    public Task<Comment> GetSingleAsync(int id)
    {
        // Pronalazimo komentar.
        Comment? existingComment =
            _comments.FirstOrDefault(c => c.Id == id);

        // Ako komentar ne postoji, prijavljujemo grešku.
        if (existingComment == null)
        {
            throw new Exception("Comment not found.");
        }

        // Vraćamo pronađeni komentar kroz Task.
        return Task.FromResult(existingComment);
    }

    // Vraćamo sve komentare.
    public IQueryable<Comment> GetMany()
    {
        // Pretvaramo listu u oblik koji interfejs zahteva.
        return _comments.AsQueryable();
    }
}
