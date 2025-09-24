using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DocumentGenerationApplication.Models.UserModel;
using DocumentGenerationApplication.Data;
using DocumentGenerationApplication.Service;

namespace DocumentGenerationApplication.Repository
{
    public class AccountRepository: IAccountRepository 
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly AppDbContext _dbContext;


        public AccountRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IUserService userService, AppDbContext dbContext) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _dbContext = dbContext;

        }


        public async Task<IdentityResult> CreateUserAsync(SignUpUserModel userModel)
        {
            var user = new ApplicationUser()
            {
                Firstname = userModel.FirstName,
                Lastname = userModel.LastName,
                UserName=userModel.Email,             
                Email = userModel.Email
             
            };
            var result = await _userManager.CreateAsync(user, userModel.Password);
            return result;

        }

        public async Task<SignInResult> PasswordSignInAsync(SignInModel SignInModel)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(SignInModel.UserName, SignInModel.Password, SignInModel.Rememberme, false);
                return result;
            }
            catch (Exception ex)
            {
                var x = ex.Message;
                return null;
            }
        }
        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();

        }

        public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordModel model)
        {
            var userId = _userService.GetUserID();
            var user = await _userManager.FindByIdAsync(userId);
            //user.PasswordExpiration = DateTime.Now.AddDays(90);
            return await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

        }

        public async Task<ApplicationUser> FindByEmailIdAsync(string email)
        {
            // Implement logic to find user by EmployeeID
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            return user;
        }

        public async Task<bool> UpdatePasswordAsync(ApplicationUser user, string newPassword)
        {
            // Implement logic to update user password
            var GeneratePasswordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            //user.PasswordExpiration = DateTime.Now.AddDays(90);
            var result = await _userManager.ResetPasswordAsync(user, GeneratePasswordResetToken, newPassword);

            return result.Succeeded;
        }

    }
}
