using System.ComponentModel.DataAnnotations;

namespace ForumSugar.DTOs
{
    public class LoginDto
    {
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;
    }
}

