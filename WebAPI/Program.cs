using Microsoft.AspNetCore.Builder; // Omogućava podešavanje Web API aplikacije.
using FileRepositories; // klase koje cuvaju json fajlove
using RepositoryContracts; //interfejsi za korisnike objave i komentare


// Kreiramo Web API aplikaciju.
var builder = WebApplication.CreateBuilder(args);

// Registrujemo podršku za kontrolere.
// Kontroleri će obrađivati HTTP zahteve.
builder.Services.AddControllers();

// Kada Web API zatraži IUserRepository,
// koristićemo UserFileRepository.
builder.Services.AddSingleton<IUserRepository, UserFileRepository>();

// Kada zatraži IPostRepository,
// koristićemo PostFileRepository.
builder.Services.AddSingleton<IPostRepository, PostFileRepository>();

// Kada zatraži ICommentRepository,
// koristićemo CommentFileRepository.
builder.Services.AddSingleton<ICommentRepository, CommentFileRepository>();

// Zadržavamo podršku za OpenAPI dokumentaciju.
builder.Services.AddOpenApi();

// Završavamo podešavanje i pravimo aplikaciju.
var app = builder.Build();

// OpenAPI dokumentacija je dostupna u Development režimu.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Omogućavamo preusmeravanje HTTP zahteva na HTTPS.
app.UseHttpsRedirection();

// Povezujemo rute sa metodama u našim kontrolerima.
app.MapControllers();

// Pokrećemo Web API server.
app.Run();