using Entities;
using RepositoryContracts;

namespace UI;

public class CliApp
{
    private readonly IUserRepository userRepository;
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;

    public CliApp(
        IUserRepository userRepository,
        IPostRepository postRepository,
        ICommentRepository commentRepository)
    {
        this.userRepository = userRepository;
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }

    public async Task StartAsync()
    {
        while (true)
        {
            Console.WriteLine("===== Forum App =====");
            Console.WriteLine("1. Create user");
            Console.WriteLine("2. Create post");
            Console.WriteLine("3. Add comment");
            Console.WriteLine("4. View posts");
            Console.WriteLine("5. View one post");
            Console.WriteLine("6. View users");
            Console.WriteLine("0. Exit");

            Console.Write("Choose option: ");
            string choice = Console.ReadLine() ?? "";

            if (choice == "1")
            {
                Console.Write("Username: ");
                string userName = Console.ReadLine() ?? "";

                Console.Write("Password: ");
                string password = Console.ReadLine() ?? "";

                User user = new User
                {
                    UserName = userName,
                    Password = password
                };

                User createdUser = await userRepository.AddAsync(user);
                Console.WriteLine($"Created user with ID: {createdUser.Id}");
            }
            else if (choice == "2")
            {
                Console.Write("Title: ");
                string title = Console.ReadLine() ?? "";

                Console.Write("Body: ");
                string body = Console.ReadLine() ?? "";

                Console.Write("User ID: ");
                int userId = int.Parse(Console.ReadLine() ?? "0");

                Post post = new Post
                {
                    Title = title,
                    Body = body,
                    UserId = userId
                };

                Post createdPost = await postRepository.AddAsync(post);
                Console.WriteLine($"Created post with ID: {createdPost.Id}");
            }
            else if (choice == "3")
            {
                Console.Write("Comment body: ");
                string body = Console.ReadLine() ?? "";

                Console.Write("User ID: ");
                int userId = int.Parse(Console.ReadLine() ?? "0");

                Console.Write("Post ID: ");
                int postId = int.Parse(Console.ReadLine() ?? "0");

                Comment comment = new Comment
                {
                    Body = body,
                    UserId = userId,
                    PostId = postId
                };

                Comment createdComment = await commentRepository.AddAsync(comment);
                Console.WriteLine($"Created comment with ID: {createdComment.Id}");
            }
            else if (choice == "4")
            {
                Console.WriteLine("Posts:");
                foreach (Post post in postRepository.GetMany())
                {
                    Console.WriteLine($"ID: {post.Id} | Title: {post.Title}");
                }
            }
            else if (choice == "5")
            {
                Console.Write("Post ID: ");
                int id = int.Parse(Console.ReadLine() ?? "0");

                Post post = await postRepository.GetSingleAsync(id);

                Console.WriteLine($"ID: {post.Id}");
                Console.WriteLine($"Title: {post.Title}");
                Console.WriteLine($"Body: {post.Body}");
                Console.WriteLine($"User ID: {post.UserId}");

                Console.WriteLine("Comments:");
                foreach (Comment comment in commentRepository.GetMany())
                {
                    if (comment.PostId == id)
                    {
                        Console.WriteLine($"Comment ID: {comment.Id}");
                        Console.WriteLine($"Body: {comment.Body}");
                    }
                }
            }
            else if (choice == "6")
            {
                Console.WriteLine("Users:");
                foreach (User user in userRepository.GetMany())
                {
                    Console.WriteLine($"ID: {user.Id} | Username: {user.UserName}");
                }
            }
            else if (choice == "0")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid option.");
            }

            Console.WriteLine();
        }
    }
}