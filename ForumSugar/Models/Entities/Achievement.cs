namespace ForumSugar.Models.Entities
{
    public class Achievement
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Description { get; set; }
        public ICollection<GetAchievement> GetAchievements { get; set; }
    }
}
