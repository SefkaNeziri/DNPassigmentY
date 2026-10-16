using System.Text.Json; // Omogućava rad sa JSON formatom.
using Entities; // Sadrži našu klasu User.
using RepositoryContracts; // Sadrži interfejs IUserRepository.

namespace FileRepositories;

public class UserFileRepository : IUserRepository
{
    // Putanja do JSON fajla.
    // AppContext.BaseDirectory omogućava da kod radi i na drugom računaru,
    // jer ne koristimo fiksnu apsolutnu putanju.
    private readonly string _filePath =
        Path.Combine(AppContext.BaseDirectory, "users.json");

    // Konstruktor: proverava da li fajl postoji.
    public UserFileRepository()
    {
        // Ako fajl ne postoji, pravimo prazan JSON niz.
        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    // Asinhrono učitavanje svih korisnika iz JSON fajla.
    private async Task<List<User>> LoadAsync()
    {
        // Čitamo ceo sadržaj fajla kao tekst.
        string json = await File.ReadAllTextAsync(_filePath);

        // Ako je fajl prazan ili sadrži samo razmake,
        // vraćamo praznu listu.
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<User>();
        }

        // Pretvaramo JSON tekst u listu User objekata.
        return JsonSerializer.Deserialize<List<User>>(json)
               ?? new List<User>();
    }

    // Asinhrono čuvanje cele liste korisnika u JSON fajl.
    private async Task SaveAsync(List<User> users)
    {
        // Pretvaramo listu korisnika u lepo formatiran JSON tekst.
        string json = JsonSerializer.Serialize(
            users,
            new JsonSerializerOptions { WriteIndented = true });

        // Upisujemo JSON tekst u fajl i čekamo da se upis završi.
        await File.WriteAllTextAsync(_filePath, json);
    }

    // Dodavanje novog korisnika.
    public async Task<User> AddAsync(User user)
    {
        // Učitavamo postojeće korisnike iz fajla.
        List<User> users = await LoadAsync();

        // Dodeljujemo novi ID.
        // Ako nema nijednog korisnika, prvi dobija ID 1.
        // Inače uzimamo najveći postojeći ID i dodajemo 1.
        user.Id = users.Count == 0
            ? 1
            : users.Max(u => u.Id) + 1;

        // Dodajemo novog korisnika u listu.
        users.Add(user);

        // Čuvamo ažuriranu listu u fajl.
        await SaveAsync(users);

        // Vraćamo novokreiranog korisnika.
        return user;
    }

    // Izmena postojećeg korisnika.
    public async Task UpdateAsync(User user)
    {
        // Učitavamo korisnike iz fajla.
        List<User> users = await LoadAsync();

        // Tražimo korisnika sa istim ID-jem.
        User? existingUser =
            users.FirstOrDefault(u => u.Id == user.Id);

        // Ako korisnik ne postoji, prijavljujemo grešku.
        if (existingUser == null)
        {
            throw new Exception("User not found.");
        }

        // Menjamo korisničko ime.
        existingUser.UserName = user.UserName;

        // Menjamo lozinku.
        existingUser.Password = user.Password;

        // Čuvamo celu ažuriranu listu u fajl.
        await SaveAsync(users);
    }

    // Brisanje korisnika po ID-ju.
    public async Task DeleteAsync(int id)
    {
        // Učitavamo sve korisnike.
        List<User> users = await LoadAsync();

        // Tražimo korisnika kojeg želimo da obrišemo.
        User? existingUser =
            users.FirstOrDefault(u => u.Id == id);

        // Ako ne postoji, prijavljujemo grešku.
        if (existingUser == null)
        {
            throw new Exception("User not found.");
        }

        // Uklanjamo korisnika iz liste.
        users.Remove(existingUser);

        // Čuvamo novu listu bez tog korisnika.
        await SaveAsync(users);
    }

    // Vraća jednog korisnika prema ID-ju.
    public async Task<User> GetSingleAsync(int id)
    {
        // Učitavamo korisnike iz JSON fajla.
        List<User> users = await LoadAsync();

        // Tražimo korisnika sa traženim ID-jem.
        User? existingUser =
            users.FirstOrDefault(u => u.Id == id);

        // Ako korisnik ne postoji, prijavljujemo grešku.
        if (existingUser == null)
        {
            throw new Exception("User not found.");
        }

        // Vraćamo pronađenog korisnika.
        return existingUser;
    }

    // Vraća sve korisnike.
    public IQueryable<User> GetMany()
    {
        // Ova metoda ne može da bude async jer interfejs
        // zahteva IQueryable<User> kao povratnu vrednost.
        // Zato sačekamo rezultat ručno.
        List<User> users = LoadAsync().GetAwaiter().GetResult();

        // Pretvaramo listu u IQueryable<User>.
        return users.AsQueryable();
    }
}