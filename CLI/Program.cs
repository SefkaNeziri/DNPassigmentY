using FileRepositories; // Naše klase koje čuvaju podatke u JSON fajlove.
using RepositoryContracts; // Interfejsi za korisnike, objave i komentare.
using UI; // Klasa CliApp koja prikazuje meni.

// Pravimo repository za korisnike.
// On čuva korisnike u users.json.
IUserRepository userRepository = new UserFileRepository();

// Pravimo repository za objave.
// On čuva objave u posts.json.
IPostRepository postRepository = new PostFileRepository();

// Pravimo repository za komentare.
// On čuva komentare u comments.json.
ICommentRepository commentRepository = new CommentFileRepository();

// Prosleđujemo sva tri repozitorijuma našoj CLI aplikaciji.
CliApp app = new CliApp(
    userRepository,
    postRepository,
    commentRepository);

// Pokrećemo aplikaciju i njen meni.
await app.StartAsync();