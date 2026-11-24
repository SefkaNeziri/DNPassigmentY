using BlazorApp.Auth;
using BlazorApp.Components;
using BlazorApp.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Registrujemo Blazor komponente i interaktivni rezim.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Omogucavamo autentifikaciju i autorizaciju u Blazoru.
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

// Registrujemo HttpClient za komunikaciju sa Web API-jem.
builder.Services.AddScoped(sp => new HttpClient
{
    // Adresa naseg Web API servera.
    BaseAddress = new Uri("http://localhost:5265/")
});

// Registrujemo servise za korisnike, objave i komentare.
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICommentService, CommentService>();

// Zadrzavamo ovu registraciju za postojecu Users stranicu.
builder.Services.AddScoped<UserService>();

// Registrujemo nas servis za prijavljivanje korisnika.
builder.Services.AddScoped<SimpleAuthProvider>();

// Blazor ce koristiti nas SimpleAuthProvider
// za proveru trenutnog korisnika.
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<SimpleAuthProvider>());

// Kreiramo aplikaciju.
var app = builder.Build();

// Podesavanja za produkcioni rezim.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Obrada nepostojecih stranica.
app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true);

// Preusmeravanje na HTTPS.
app.UseHttpsRedirection();

// Zastita formulara.
app.UseAntiforgery();

// Omogucavamo staticke fajlove.
app.MapStaticAssets();

// Pokrecemo interaktivnu Blazor aplikaciju.
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();