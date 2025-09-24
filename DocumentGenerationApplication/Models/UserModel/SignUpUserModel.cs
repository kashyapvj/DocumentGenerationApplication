using System.ComponentModel.DataAnnotations;

namespace DocumentGenerationApplication.Models.UserModel
{
    public class SignUpUserModel
    {

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter first name")]
        [Display(Name = "First Name")]
        public String FirstName { get; set; }=string.Empty;

        [Required(ErrorMessage = "Enter last name")]
        [Display(Name = "Last Name")]
        public String LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter email")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage ="Please enter a valid email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter password")]
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }=string.Empty;


        [Required(ErrorMessage = "Enter confirm password")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }= string.Empty;


    }
}
