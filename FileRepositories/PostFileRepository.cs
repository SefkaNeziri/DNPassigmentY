
using System.Text.Json; // Omogućava pretvaranje C# objekata u JSON i obrnuto.
using Entities; // Omogućava korišćenje naše klase Post.
using RepositoryContracts; // Omogućava korišćenje interfejsa IPostRepository.

namespace FileRepositories; // Namespace u kojem se nalazi ova klasa.

public class PostFileRepository : IPostRepository
{
    // Lista objava koje trenutno imamo u memoriji.
    private List<Post> _posts = new List<Post>();

    // Putanja do JSON fajla u kojem čuvamo objave.
    // AppContext.BaseDirectory je folder iz kojeg se pokreće aplikacija.
    // Path.Combine spaja taj folder i naziv fajla.
    private readonly string _filePath =
        Path.Combine(AppContext.BaseDirectory, "posts.json");

    // Konstruktor se izvršava kada napravimo novi PostFileRepository.
    public PostFileRepository()
    {
        // Proveravamo da li posts.json već postoji.
        if (File.Exists(_filePath))
        {
            // Čitamo sav tekst iz fajla.
            string json = File.ReadAllText(_filePath);

            // Ako je fajl prazan, nemamo šta da učitamo.
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            // Pretvaramo JSON tekst u listu Post objekata.
            // Ako Deserialize vrati null, koristimo praznu listu.
            _posts = JsonSerializer.Deserialize<List<Post>>(json)
                     ?? new List<Post>();
        }
    }

    // Pomoćna metoda koja čuva sve objave u fajl.
    // private znači da je koristimo samo unutar ove klase.
    private void Save()
    {
        // Pretvaramo listu objava u JSON tekst.
        // WriteIndented = true pravi čitljiv JSON u više redova.
        string json = JsonSerializer.Serialize(
            _posts,
            new JsonSerializerOptions { WriteIndented = true });

        // Upisujemo JSON tekst u posts.json.
        // Ako fajl ne postoji, File.WriteAllText će ga napraviti.
        File.WriteAllText(_filePath, json);
    }

    // AddAsync dodaje novu objavu.
    // Task<Post> znači da metoda vraća Post kroz Task.
    public Task<Post> AddAsync(Post post)
    {
        // Ako nema objava, prva dobija ID 1.
        // Inače uzimamo najveći postojeći ID i dodajemo 1.
        post.Id = _posts.Count == 0
            ? 1
            : _posts.Max(p => p.Id) + 1;

        // Dodajemo novu objavu u našu listu.
        _posts.Add(post);

        // Čuvamo ažuriranu listu u JSON fajl.
        Save();

        // Vraćamo upravo dodatu objavu.
        return Task.FromResult(post);
    }

    // UpdateAsync menja postojeću objavu.
    public Task UpdateAsync(Post post)
    {
        // Tražimo objavu koja ima isti ID kao prosleđena objava.
        Post? existingPost =
            _posts.FirstOrDefault(p => p.Id == post.Id);

        // Ako objava ne postoji, prijavljujemo grešku.
        if (existingPost == null)
        {
            throw new Exception("Post not found.");
        }

        // Menjamo naslov postojeće objave.
        existingPost.Title = post.Title;

        // Menjamo tekst postojeće objave.
        existingPost.Body = post.Body;

        // Menjamo ID korisnika kojem objava pripada.
        existingPost.UserId = post.UserId;

        // Čuvamo izmene u posts.json.
        Save();

        // Metoda ne vraća objavu, samo završen Task.
        return Task.CompletedTask;
    }

    // DeleteAsync briše objavu prema njenom ID-u.
    public Task DeleteAsync(int id)
    {
        // Pronalazimo objavu koju želimo da obrišemo.
        Post? existingPost =
            _posts.FirstOrDefault(p => p.Id == id);

        // Ako objava ne postoji, prijavljujemo grešku.
        if (existingPost == null)
        {
            throw new Exception("Post not found.");
        }

        // Uklanjamo pronađenu objavu iz liste.
        _posts.Remove(existingPost);

        // Čuvamo listu bez obrisane objave.
        Save();

        // Vraćamo završen Task.
        return Task.CompletedTask;
    }

    // GetSingleAsync vraća jednu objavu prema ID-u.
    public Task<Post> GetSingleAsync(int id)
    {
        // Tražimo objavu sa odgovarajućim ID brojem.
        Post? existingPost =
            _posts.FirstOrDefault(p => p.Id == id);

        // Ako nije pronađena, prijavljujemo grešku.
        if (existingPost == null)
        {
            throw new Exception("Post not found.");
        }

        // Vraćamo pronađenu objavu kroz Task.
        return Task.FromResult(existingPost);
    }

    // GetMany vraća sve objave.
    public IQueryable<Post> GetMany()
    {
        // Pretvaramo listu u IQueryable<Post>.
        // To je tip koji naš interfejs zahteva.
        return _posts.AsQueryable();
    }
}
