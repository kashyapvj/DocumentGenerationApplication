using System.ComponentModel.DataAnnotations;

namespace DocumentGenerationApplication.Models.Account
{
    public class ResetPasswordModel
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }= string.Empty;
    }

}
