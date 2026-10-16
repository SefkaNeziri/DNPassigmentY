
using System.Text.Json; // Omogućava pretvaranje C# objekata u JSON i obrnuto.
using Entities; // Omogućava korišćenje naše klase Post.
using RepositoryContracts; // Sadrži interfejs IPostRepository.

namespace FileRepositories;

// Ova klasa čuva, učitava, menja i briše objave iz JSON fajla.
// Implementira sve metode koje zahteva interfejs IPostRepository.
public class PostFileRepository : IPostRepository
{
    // Putanja do fajla u kojem čuvamo objave.
    // AppContext.BaseDirectory označava folder pokrenute aplikacije.
    // Tako ne moramo da koristimo putanju vezanu za naš računar.
    private readonly string _filePath =
        Path.Combine(AppContext.BaseDirectory, "posts.json");

    // Konstruktor se izvršava kada napravimo PostFileRepository.
    public PostFileRepository()
    {
        // Proveravamo da li JSON fajl postoji.
        if (!File.Exists(_filePath))
        {
            // Ako ne postoji, pravimo ga sa praznom JSON listom.
            // Zagrade [] predstavljaju praznu listu u JSON formatu.
            File.WriteAllText(_filePath, "[]");
        }
    }

    // Pomoćna metoda za asinhrono učitavanje objava iz fajla.
    private async Task<List<Post>> LoadAsync()
    {
        // Čitamo sav tekst iz JSON fajla.
        // await sačeka da se asinhrono čitanje završi.
        string json = await File.ReadAllTextAsync(_filePath);

        // Ako je fajl prazan, vraćamo praznu listu.
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<Post>();
        }

        // Pretvaramo JSON tekst u listu Post objekata.
        // Ako deserializacija vrati null, koristimo praznu listu.
        return JsonSerializer.Deserialize<List<Post>>(json)
               ?? new List<Post>();
    }

    // Pomoćna metoda za asinhrono čuvanje cele liste objava.
    private async Task SaveAsync(List<Post> posts)
    {
        // Pretvaramo C# listu objava u JSON tekst.
        // WriteIndented = true pravi čitljiv JSON u više redova.
        string json = JsonSerializer.Serialize(
            posts,
            new JsonSerializerOptions { WriteIndented = true });

        // Upisujemo JSON tekst u fajl.
        // await sačeka da se upisivanje završi.
        await File.WriteAllTextAsync(_filePath, json);
    }

    // AddAsync dodaje novu objavu.
    // Task<Post> znači da metoda asinhrono vraća Post objekat.
    public async Task<Post> AddAsync(Post post)
    {
        // Učitavamo sve postojeće objave iz fajla.
        List<Post> posts = await LoadAsync();

        // Novoj objavi dodeljujemo jedinstveni ID.
        // Ako lista nema objava, ID je 1.
        // Inače uzimamo najveći ID i dodajemo 1.
        post.Id = posts.Count == 0
            ? 1
            : posts.Max(p => p.Id) + 1;

        // Dodajemo novu objavu u listu.
        posts.Add(post);

        // Čuvamo ažuriranu listu objava u JSON fajl.
        await SaveAsync(posts);

        // Vraćamo novokreiranu objavu sa dodeljenim ID brojem.
        return post;
    }

    // UpdateAsync menja postojeću objavu.
    public async Task UpdateAsync(Post post)
    {
        // Učitavamo postojeće objave iz JSON fajla.
        List<Post> posts = await LoadAsync();

        // Pronalazimo objavu čiji ID odgovara prosleđenom ID-ju.
        Post? existingPost =
            posts.FirstOrDefault(p => p.Id == post.Id);

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

        // Čuvamo celu izmenjenu listu objava u JSON fajl.
        await SaveAsync(posts);

        // Nema return naredbe jer je povratni tip async Task.
    }

    // DeleteAsync briše objavu prema njenom ID broju.
    public async Task DeleteAsync(int id)
    {
        // Učitavamo objave iz JSON fajla.
        List<Post> posts = await LoadAsync();

        // Tražimo objavu koju želimo da obrišemo.
        Post? existingPost =
            posts.FirstOrDefault(p => p.Id == id);

        // Ako objava ne postoji, prijavljujemo grešku.
        if (existingPost == null)
        {
            throw new Exception("Post not found.");
        }

        // Uklanjamo pronađenu objavu iz liste.
        posts.Remove(existingPost);

        // Čuvamo listu bez obrisane objave.
        await SaveAsync(posts);

        // Nema return naredbe jer metoda samo završava Task.
    }

    // GetSingleAsync vraća jednu objavu prema ID broju.
    public async Task<Post> GetSingleAsync(int id)
    {
        // Asinhrono učitavamo objave iz JSON fajla.
        List<Post> posts = await LoadAsync();

        // Tražimo objavu sa odgovarajućim ID brojem.
        Post? existingPost =
            posts.FirstOrDefault(p => p.Id == id);

        // Ako ne pronađemo objavu, prijavljujemo grešku.
        if (existingPost == null)
        {
            throw new Exception("Post not found.");
        }

        // Vraćamo pronađenu objavu.
        // Pošto je metoda async Task<Post>, C# sam pravi Task.
        return existingPost;
    }

    // GetMany vraća sve objave iz JSON fajla.
    public IQueryable<Post> GetMany()
    {
        // Interfejs zahteva IQueryable<Post>, a ne Task.
        // Zato ova metoda ne može direktno koristiti await.
        // GetAwaiter().GetResult() sačeka da učitavanje završi.
        List<Post> posts =
            LoadAsync().GetAwaiter().GetResult();

        // Pretvaramo listu u IQueryable<Post>.
        // Tako kontroleri mogu da koriste LINQ metode kao Where().
        return posts.AsQueryable();
    }
}
