using System.ComponentModel.DataAnnotations;

namespace DocumentGenerationApplication.Models.UserModel
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Please enter email")]
        [Display(Name = "Email")]
        [StringLength(40, ErrorMessage = "Email must be atmost 40 characters in length")]
        public string Email { get; set; }= string.Empty;

        [Required, DataType(DataType.Password), Display(Name = "New Password")]
        [Compare("ConfirmPassword", ErrorMessage = "new password and confirm password does not match")]
        public string Password { get; set; }=string.Empty;

        [Required, DataType(DataType.Password), Display(Name = "Confirm New Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
