namespace ForumSugar.Models.Entities
{
    public class GetAchievement
    {
        public int Id { get; set; }  // Khóa chính
        public int UserId { get; set; }  // Người đạt được thành tựu
        public User User { get; set; }  // Người đạt thành tựu
        public int AchievementId { get; set; }  // Thành tựu đạt được
        public Achievement Achievement { get; set; }  // Thành tựu
        public DateTime AchievedAt { get; set; }  // Thời gian đạt thành tựu
    }
}
