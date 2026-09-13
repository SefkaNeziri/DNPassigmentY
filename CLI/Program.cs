using InMemoryRepositories;
using RepositoryContracts;
using UI;
using RepositoryContracts;

IUserRepository userRepository = new UserInMemoryRepository();
IPostRepository postRepository = new PostInMemoryRepository();
ICommentRepository commentRepository = new CommentInMemoryRepository();

CliApp app = new CliApp(
    userRepository,
    postRepository,
    commentRepository);

await app.StartAsync();