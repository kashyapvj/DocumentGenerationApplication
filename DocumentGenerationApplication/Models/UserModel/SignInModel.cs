using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentGenerationApplication.Models.UserModel
{
    public class SignInModel
    {
        [Required(ErrorMessage = "Please enter username")]
        [Display(Name = "UserName")]
        public string UserName { get; set; }= string.Empty;

        [Required(ErrorMessage = "Please enter password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }=string.Empty;

        [Display(Name ="Remember Me")]
        public bool Rememberme { get; set; }




    }
}
