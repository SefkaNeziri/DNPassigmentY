using System.Text.Json; // Omogućava rad sa JSON formatom.
using Entities; // Sadrži klasu Comment.
using RepositoryContracts; // Sadrži interfejs ICommentRepository.

namespace FileRepositories;

public class CommentFileRepository : ICommentRepository
{
    // Putanja do JSON fajla u kojem čuvamo komentare.
    private readonly string _filePath =
        Path.Combine(AppContext.BaseDirectory, "comments.json");

    // Konstruktor: pravi prazan JSON fajl ako ne postoji.
    public CommentFileRepository()
    {
        if (!File.Exists(_filePath))
        {
            // [] predstavlja praznu listu u JSON formatu.
            File.WriteAllText(_filePath, "[]");
        }
    }

    // Asinhrono učitavanje svih komentara iz fajla.
    private async Task<List<Comment>> LoadAsync()
    {
        // Čitamo sadržaj comments.json fajla.
        string json = await File.ReadAllTextAsync(_filePath);

        // Ako je fajl prazan, vraćamo praznu listu.
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<Comment>();
        }

        // Pretvaramo JSON tekst u listu Comment objekata.
        return JsonSerializer.Deserialize<List<Comment>>(json)
               ?? new List<Comment>();
    }

    // Asinhrono čuvanje cele liste komentara u JSON fajl.
    private async Task SaveAsync(List<Comment> comments)
    {
        // Pretvaramo listu komentara u lepo formatiran JSON tekst.
        string json = JsonSerializer.Serialize(
            comments,
            new JsonSerializerOptions { WriteIndented = true });

        // Asinhrono upisujemo JSON tekst u fajl.
        await File.WriteAllTextAsync(_filePath, json);
    }

    // Dodavanje novog komentara.
    public async Task<Comment> AddAsync(Comment comment)
    {
        // Učitavamo postojeće komentare.
        List<Comment> comments = await LoadAsync();

        // Dodeljujemo novi ID.
        // Ako nema komentara, prvi dobija ID 1.
        // Inače uzimamo najveći postojeći ID i dodajemo 1.
        comment.Id = comments.Count == 0
            ? 1
            : comments.Max(c => c.Id) + 1;

        // Dodajemo novi komentar u listu.
        comments.Add(comment);

        // Čuvamo ažuriranu listu u fajl.
        await SaveAsync(comments);

        // Vraćamo novododati komentar.
        return comment;
    }

    // Izmena postojećeg komentara.
    public async Task UpdateAsync(Comment comment)
    {
        // Učitavamo postojeće komentare.
        List<Comment> comments = await LoadAsync();

        // Tražimo komentar sa istim ID-jem.
        Comment? existingComment =
            comments.FirstOrDefault(c => c.Id == comment.Id);

        // Ako komentar ne postoji, prijavljujemo grešku.
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

        // Čuvamo celu izmenjenu listu komentara.
        await SaveAsync(comments);
    }

    // Brisanje komentara po ID-ju.
    public async Task DeleteAsync(int id)
    {
        // Učitavamo postojeće komentare.
        List<Comment> comments = await LoadAsync();

        // Tražimo komentar koji treba obrisati.
        Comment? existingComment =
            comments.FirstOrDefault(c => c.Id == id);

        // Ako komentar ne postoji, prijavljujemo grešku.
        if (existingComment == null)
        {
            throw new Exception("Comment not found.");
        }

        // Uklanjamo komentar iz liste.
        comments.Remove(existingComment);

        // Čuvamo novu listu bez tog komentara.
        await SaveAsync(comments);
    }

    // Vraća jedan komentar prema ID-ju.
    public async Task<Comment> GetSingleAsync(int id)
    {
        // Učitavamo komentare iz JSON fajla.
        List<Comment> comments = await LoadAsync();

        // Tražimo komentar sa traženim ID-jem.
        Comment? existingComment =
            comments.FirstOrDefault(c => c.Id == id);

        // Ako komentar nije pronađen, prijavljujemo grešku.
        if (existingComment == null)
        {
            throw new Exception("Comment not found.");
        }

        // Vraćamo pronađeni komentar.
        return existingComment;
    }

    // Vraća sve komentare.
    public IQueryable<Comment> GetMany()
    {
        // Ova metoda ne može direktno koristiti await
        // jer interfejs zahteva IQueryable<Comment>.
        // Zato ručno sačekamo rezultat asinhrone metode.
        List<Comment> comments =
            LoadAsync().GetAwaiter().GetResult();

        // Pretvaramo listu u IQueryable<Comment>.
        return comments.AsQueryable();
    }
}