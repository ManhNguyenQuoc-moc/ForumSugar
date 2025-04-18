
namespace ForumSugar.Models.Entities
    {
        public class Statistic
        {
            public int Id { get; set; }
            public int TotalUsers { get; set; }
            public int TotalPosts { get; set; }
            public int TotalTopics { get; set; }
            public int TotalComments { get; set; }
            public DateTime LastUpdated { get; set; }
        }
    }


