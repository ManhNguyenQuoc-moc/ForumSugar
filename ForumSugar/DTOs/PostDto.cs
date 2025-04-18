namespace ForumSugar.DTOs
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        // Thông tin người dùng
        public int UserId { get; set; }
        public string AuthorName { get; set; }
        public string AvatarUrl { get; set; }

        // Tổng like và comment
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }

        // Trạng thái người dùng hiện tại
        public bool IsLikedByCurrentUser
        {
            get; set;
        }
    }
}
