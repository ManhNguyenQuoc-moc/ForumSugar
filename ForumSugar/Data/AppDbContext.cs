using ForumSugar.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForumSugar.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostReport> PostReports { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<CommentReport> CommentReports { get; set; }
        public DbSet<FeedbackChatbot> FeedbackChatBots { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<GetAchievement> GetAchievements { get; set; }
        public DbSet<Ranking> Rankings { get; set; }
        public DbSet<SavedPost> savedPosts { get; set; }
        public DbSet<Topic> topics { get; set; }
        public DbSet<UserFollow> userFollows { get; set; }
        public DbSet<ChatHistory> chatHistories { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { // User - Post (1:N)
            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)  // Mỗi bài viết có một người dùng
                .WithMany(u => u.Posts)  // Một người dùng có nhiều bài viết
                .HasForeignKey(p => p.UserId)  // Khoá ngoại liên kết với UserId
                .OnDelete(DeleteBehavior.Restrict);  // Không xóa bài viết khi người dùng bị xóa

            // User - Comment (1:N)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)  // Mỗi bình luận thuộc về một người dùng
                .WithMany(u => u.Comments)  // Một người dùng có thể có nhiều bình luận
                .HasForeignKey(c => c.UserId)  // Khoá ngoại liên kết với UserId
                .OnDelete(DeleteBehavior.Restrict);  // Không xóa bình luận khi người dùng bị xóa

            // Post - Comment (1:N)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)  // Mỗi bình luận thuộc về một bài viết
                .WithMany(p => p.Comments)  // Một bài viết có thể có nhiều bình luận
                .HasForeignKey(c => c.PostId)  // Khoá ngoại liên kết với PostId
                .OnDelete(DeleteBehavior.Restrict);  // Không xóa bình luận khi bài viết bị xóa

            // Post - PostReport (1:N)
            modelBuilder.Entity<PostReport>()
                .HasOne(pr => pr.Post)  // Mỗi báo cáo liên quan đến một bài viết
                .WithMany(p => p.Reports)  // Một bài viết có thể bị nhiều báo cáo
                .HasForeignKey(pr => pr.PostId)  // Khoá ngoại liên kết với PostId
                .OnDelete(DeleteBehavior.Cascade);  // Xóa báo cáo khi bài viết bị xóa

            // User - Notification (Sender) (1:N)
            modelBuilder.Entity<User>()
                .HasMany(u => u.SentNotifications)  // Một người dùng có thể gửi nhiều thông báo
                .WithOne(n => n.Sender)  // Mỗi thông báo có một người gửi
                .HasForeignKey(n => n.SenderId)  // Khoá ngoại liên kết với SenderId
                .OnDelete(DeleteBehavior.Restrict);  // Không xóa thông báo khi người gửi bị xóa

            // User - Notification (Receiver) (1:N)
            modelBuilder.Entity<User>()
                .HasMany(u => u.ReceivedNotifications)  // Một người dùng có thể nhận nhiều thông báo
                .WithOne(n => n.Receiver)  // Mỗi thông báo có một người nhận
                .HasForeignKey(n => n.ReceiverId)  // Khoá ngoại liên kết với ReceiverId
                .OnDelete(DeleteBehavior.Restrict);  // Không xóa thông báo khi người nhận bị xóa

            // Notification - NotificationType (Enum)
            modelBuilder.Entity<Notification>()
                .Property(n => n.Type)
                .HasConversion<string>();  // Lưu dưới dạng chuỗi để dễ dàng đọc từ cơ sở dữ liệu
            // UserFollow - User (N:1) (Mỗi người dùng có thể theo dõi nhiều người và có thể bị nhiều người theo dõi)
            // Cấu hình quan hệ User - UserFollow (Người dùng đang theo dõi)
            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.Following)  // Mỗi bản ghi UserFollow có một người được theo dõi
                .WithMany(u => u.Following)  // Một người dùng có thể theo dõi nhiều người
                .HasForeignKey(uf => uf.FollowingId); // Khoá ngoại liên kết với FollowingId
            // Cấu hình quan hệ User - UserFollow (Người dùng bị theo dõi)
            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.Follower)  // Mỗi bản ghi UserFollow có một người theo dõi
                .WithMany(u => u.Followers)  // Một người dùng có thể bị theo dõi bởi nhiều người
                .HasForeignKey(uf => uf.FollowerId)  // Khoá ngoại liên kết với FollowerId
                .OnDelete(DeleteBehavior.Restrict);  // Không xóa người dùng khi bị theo dõi bị xóa // Không xóa UserFollow khi người dùng bị xóa

            // SavedPost - Post (N:1) (Một người dùng có thể lưu nhiều bài viết)
            // Cấu hình quan hệ User - SavedPost
            modelBuilder.Entity<SavedPost>()
               .HasOne(sp => sp.User)  // Mỗi SavedPost thuộc về một người dùng
               .WithMany(u => u.SavedPosts)  // Một người dùng có thể lưu nhiều bài viết
               .HasForeignKey(sp => sp.UserId);  // Khoá ngoại liên kết với UserId

            modelBuilder.Entity<SavedPost>()
                .HasOne(sp => sp.Post)  // Mỗi SavedPost liên kết với một bài viết
                .WithMany()  // Một bài viết có thể được nhiều người dùng lưu
                .HasForeignKey(sp => sp.PostId);  // Khoá ngoại liên kết với PostId   
            // User - FeedbackChatbot (1:N)
            modelBuilder.Entity<FeedbackChatbot>()
                .HasOne(fc => fc.User)  // Mỗi phản hồi của chatbot có một người dùng
                .WithMany(u => u.FeedbackChatBots)  // Một người dùng có thể gửi nhiều phản hồi
                .HasForeignKey(fc => fc.UserId)  // Khoá ngoại liên kết với UserId
                .OnDelete(DeleteBehavior.Restrict);  // Không xóa phản hồi khi người dùng bị xóa

         

            // Achievement - GetAchievement (1:N)
            modelBuilder.Entity<GetAchievement>()
                .HasOne(ga => ga.Achievement)  // Mỗi bản ghi GetAchievement có một thành tựu
                .WithMany(a => a.GetAchievements)  // Một thành tựu có thể có nhiều người đạt được
                .HasForeignKey(ga => ga.AchievementId)  // Khoá ngoại liên kết với AchievementId
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Post>()
                .Property(p => p.Likes)
                .HasConversion(
                    v => string.Join(",", v),       // Lưu: List<int> → string
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(int.Parse).ToList() // Load: string → List<int>
                );

            base.OnModelCreating(modelBuilder);// Không xóa GetAchievement khi thành tựu bị xóa
        }
    }
}
