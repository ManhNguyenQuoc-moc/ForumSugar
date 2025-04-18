namespace ForumSugar.Models.Entities
{
    public class FeedbackChatbot
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
