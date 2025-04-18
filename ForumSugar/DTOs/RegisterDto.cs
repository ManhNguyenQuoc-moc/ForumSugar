namespace ForumSugar.DTOs
{
    public class RegisterDto
    {
        public string Name { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateTime? Bornday { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? ConfirmPassword { get; set; } = null!;
    }
}
