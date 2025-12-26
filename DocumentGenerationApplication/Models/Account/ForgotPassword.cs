using System.ComponentModel.DataAnnotations;

namespace DocumentGenerationApplication.Models.Account
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }=string.Empty;
    }

}
