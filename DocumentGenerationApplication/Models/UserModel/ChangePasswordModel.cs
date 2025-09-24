using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentGenerationApplication.Models.UserModel
{
    public class ChangePasswordModel
    {
        [Required, DataType(DataType.Password), Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }= string.Empty;

        [Required, DataType(DataType.Password), Display(Name = "New Password")]
        [Compare("ConfirmNewPassword", ErrorMessage = "New Password and Confirm Password does not match")]
        public string NewPassword { get; set; }=string.Empty;

        [Required, DataType(DataType.Password), Display(Name = "Confirm New Password")]

        public string ConfirmNewPassword { get; set; } = string.Empty;

    }
}
