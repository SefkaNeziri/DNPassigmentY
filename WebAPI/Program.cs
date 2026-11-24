using EfcRepositories;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

var builder = WebApplication.CreateBuilder(args);

// Registrujemo Web API kontrolere.
builder.Services.AddControllers();

// Dodajemo podrsku za OpenAPI dokumentaciju.
builder.Services.AddOpenApi();

// Registrujemo DbContext i povezujemo ga sa SQLite bazom.
builder.Services.AddDbContext<ForumDbContext>(options =>
    options.UseSqlite(ForumDatabase.GetConnectionString()));

// Registrujemo nove Entity Framework Core repozitorijume.
// Oni zamenjuju prethodne repozitorijume koji su koristili JSON.
builder.Services.AddScoped<IUserRepository, EfcUserRepository>();
builder.Services.AddScoped<IPostRepository, EfcPostRepository>();
builder.Services.AddScoped<ICommentRepository, EfcCommentRepository>();

// Kreiramo aplikaciju.
var app = builder.Build();

// OpenAPI dokumentacija dostupna je u Development rezimu.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Omogucavamo HTTPS preusmeravanje.
app.UseHttpsRedirection();

// Povezujemo HTTP adrese sa metodama u kontrolerima.
app.MapControllers();

// Pokrecemo Web API.
app.Run();