namespace ForumSugar.Models.Entities
{
    public class CommentReport
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
        public int CommentId { get; set; }
        public Comment Comment { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
