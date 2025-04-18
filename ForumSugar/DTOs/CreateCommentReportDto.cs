namespace ForumSugar.DTOs
{
    public class CreateCommentReportDto
    {
        public int CommentId { get; set; }
        public string Reason { get; set; }
        public int UserId { get; set; } // Hoặc lấy từ token
    }
}
