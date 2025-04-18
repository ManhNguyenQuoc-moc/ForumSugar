namespace ForumSugar.DTOs
{
    public class CreatePostDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }

        public int UserId { get; set; }
        public int TopicId { get; set; }
    }
}
