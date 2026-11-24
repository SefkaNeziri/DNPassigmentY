namespace EfcRepositories;

// Ova klasa odredjuje gde ce se cuvati SQLite baza podataka.
public static class ForumDatabase
{
    // Metoda vraca connection string za nasu bazu.
    public static string GetConnectionString()
    {
        // Pronalazimo lokalni folder trenutnog korisnika.
        string folder = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "DNPassigmentY");

        // Ako folder ne postoji, automatski ga pravimo.
        Directory.CreateDirectory(folder);

        // Odredjujemo naziv i putanju baze podataka.
        string databasePath = Path.Combine(folder, "forum.db");

        // Vracamo podatke potrebne za povezivanje sa SQLite bazom.
        return $"Data Source={databasePath}";
    }
}