

namespace ForumSugar.Models.Entities
{
    public class SavedPost
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int PostId { get; set; }
        public DateTime SavedAt { get; set; }
        public User User { get; set; }
        public Post Post { get; set; }
    }
}
