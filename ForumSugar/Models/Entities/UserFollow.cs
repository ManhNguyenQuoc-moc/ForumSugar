namespace ForumSugar.Models.Entities
{
    public class UserFollow
    {
        public int Id { get; set; } 

        public int FollowerId { get; set; }  
        public User Follower { get; set; }  // Tham chiếu đến người theo dõi

        public int FollowingId { get; set; }  // Người được theo dõi
        public User Following { get; set; }  // Tham chiếu đến người được theo dõi

    }
}
