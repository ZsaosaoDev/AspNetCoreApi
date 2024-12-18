using System.ComponentModel.DataAnnotations;

namespace Asp.NETCoreApi.DTOs {
    public class SignInDto {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
