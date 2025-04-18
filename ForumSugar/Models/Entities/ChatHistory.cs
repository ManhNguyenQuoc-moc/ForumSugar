namespace ForumSugar.Models.Entities
{
    public class ChatHistory
    {
        public int Id { get; set; }
        public string UserQuestion { get; set; }
        public string BotAnswer { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
