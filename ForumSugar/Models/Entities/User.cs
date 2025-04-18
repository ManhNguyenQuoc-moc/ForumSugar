
using ForumSugar.Models.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }  // Họ và tên
    public string Email { get; set; }
    public string Username { get; set;}
    public string PasswordHash { get; set; }
    public string Role { get; set; } = "User";
    public bool Gender { get; set; }
    public DateTime? Bornday { get; set; }
    public string PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsLocked { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Post> Posts { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<Notification> SentNotifications { get; set; }
    public ICollection<Notification> ReceivedNotifications { get; set; }
    public ICollection<SavedPost> SavedPosts { get; set; }
    public ICollection<UserFollow> Following { get; set; }  
    public ICollection<FeedbackChatbot> FeedbackChatBots { get; set; }
    public ICollection<UserFollow> Followers { get; set; }
    public ICollection<GetAchievement> GetAchievements { get; set; }

}
