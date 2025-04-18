namespace ForumSugar.Models.Entities
{
    public class Notification
    {

        public int Id { get; set; }  // Khóa chính
        public int SenderId { get; set; }  // Người gửi thông báo
        public User Sender { get; set; }  // Người gửi thông báo

        public int ReceiverId { get; set; }  // Người nhận thông báo
        public User Receiver { get; set; }  // Người nhận thông báo

        public string Content { get; set; }  // Nội dung thông báo

        public DateTime CreatedAt { get; set; }  // Thời gian tạo thông báo

        public bool IsRead { get; set; }  // Trạng thái đã đọc hay chưa (True/False)

        public NotificationType Type { get; set; }
    }
    public enum NotificationType
    {
        System,       
        NewComment,   
        NewReaction,  
        NewFollow,    
        NewMessage,   
        NewAchievement 
    }

}
