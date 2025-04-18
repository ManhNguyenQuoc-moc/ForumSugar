using System;

namespace ForumSugar.Models.Entities
{
    public class PostReport
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
        // Liên kết đến Blog
        public int PostId { get; set; }
        public Post Post { get; set; }
        // Liên kết đến User (người báo cáo)
        public int UseId { get; set; }
        public User User { get; set; }
    }
}
