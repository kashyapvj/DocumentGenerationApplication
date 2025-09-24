using Microsoft.AspNetCore.Identity;

namespace DocumentGenerationApplication.Models.UserModel
{
    public class ApplicationUser : IdentityUser
    {
    
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; }= string.Empty; 

    }
}
