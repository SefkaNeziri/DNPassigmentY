using BlazorApp.Components;
using BlazorApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Registrujemo Blazor komponente i interaktivni režim.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Registrujemo HttpClient za komunikaciju sa Web API-jem.
builder.Services.AddScoped(sp => new HttpClient
{
    // Adresa servera koji smo napravile u Part 4.
    BaseAddress = new Uri("http://localhost:5265/")
});

// Registrujemo servise preko njihovih interfejsa.
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICommentService, CommentService>();

// Ostavljamo i ovu registraciju jer je koristi postojeći Users.razor.
builder.Services.AddScoped<UserService>();

// Pravimo aplikaciju.
var app = builder.Build();

// Podešavanja kada aplikacija nije u razvojnom režimu.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Podešavamo stranice za greške i HTTPS.
app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true);

app.UseHttpsRedirection();

// Zaštita obrazaca.
app.UseAntiforgery();

// Statički fajlovi aplikacije.
app.MapStaticAssets();

// Pokrećemo Blazor sa interaktivnim komponentama.
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();