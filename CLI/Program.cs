using FileRepositories;
using InMemoryRepositories;
using RepositoryContracts;
using UI;
using RepositoryContracts;

IUserRepository userRepository = new UserFileRepository();
IPostRepository postRepository = new PostInMemoryRepository();
ICommentRepository commentRepository = new CommentInMemoryRepository();

CliApp app = new CliApp(
    userRepository,
    postRepository,
    commentRepository);

await app.StartAsync();