
using DocumentGenerationApplication.Models.UserModel;
using Microsoft.AspNetCore.Identity;

namespace DocumentGenerationApplication.Repository
{
    public interface IAccountRepository
    {

        Task<IdentityResult> CreateUserAsync(SignUpUserModel userModel);
        Task<SignInResult> PasswordSignInAsync(SignInModel SignInModel);
        Task SignOutAsync();
        Task<IdentityResult> ChangePasswordAsync(ChangePasswordModel model);

        Task<ApplicationUser> FindByEmailIdAsync(string employeeID);
        Task<bool> UpdatePasswordAsync(ApplicationUser user, string newPassword);

    }
}