

namespace ForumSugar.Models.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public string ImageUrl { get; set; }  
        public bool isAproved { get; set; }
        public bool isLocked { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        
        public ICollection<Comment> Comments { get; set; }
        public ICollection<PostReport> Reports { get; set; }
        public int TopicId { get; set; }

        public Topic Topic { get; set; }
        // Danh sách ID người đã like, sẽ lưu dạng JSON string trong DB
        public List<int> Likes { get; set; } = new List<int>();
    }

}
