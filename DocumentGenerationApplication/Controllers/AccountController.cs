using DocumentGenerationApplication.Models.Account;
using DocumentGenerationApplication.Models.UserModel;
using DocumentGenerationApplication.Repository;
using DocumentGenerationApplication.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace DocumentGenerationApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEmailService _emailService;


        public AccountController(IAccountRepository accountRepository, IEmailService emailService)
        {
            _accountRepository = accountRepository;
            _emailService = emailService;
        }


        [Route("Signup")]
        //[Authorize(Roles = "Admin")]
        public IActionResult SignUp()
        {
            return View();
        }

      

        [HttpPost]
        [Route("Signup")]
        public async Task<IActionResult> SignUp(SignUpUserModel UserModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.CreateUserAsync(UserModel);

                if (!result.Succeeded)
                {
                    foreach (var errorMessage in result.Errors)
                    {
                        if (errorMessage.Code == "DuplicateUserName")
                        {
                            ModelState.AddModelError("", "Email already exist.");
                        }
                        else
                        {
                            ModelState.AddModelError("", errorMessage.Description);
                        }
                    }

                    // Set failure alert
                    ViewBag.SignUpError = "An account with this email already exists. Please enter another email.";
                }
                else
                {
                    ModelState.Clear();
                    ViewBag.SignUpAlert = "SignUp Successful!!";
                }
            }
            return View();
        }



        [HttpGet]
        public IActionResult Login()
        {

            return View();
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(SignInModel model, string? returnUrl)
        {

            if (ModelState.IsValid)
            {

                var result = await _accountRepository.PasswordSignInAsync(model);

                if ( result != null && result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return LocalRedirect(returnUrl);

                    }

                    ViewBag.LoginMessage = "Login successfully!";
                    ModelState.Clear();
                    return RedirectToAction("GetOfferLetters", "Employee");

                }
                ModelState.AddModelError("", "Invalid Credentials");
            }

            return View(model);
        }


        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _accountRepository.SignOutAsync();
            //return RedirectToAction("Index", "Home");

            return RedirectToAction("Login", "Account");

        }


        [Route("change-password")]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.ChangePasswordAsync(model);
                if (result.Succeeded)
                {
                    ViewBag.IsSuccess = true;
                    ModelState.Clear();
                    return View();
                }
                foreach (var errorMessage in result.Errors)
                {
                    ModelState.AddModelError("", errorMessage.Description);
                }
            }
            return View(model);
        }



        ////[Authorize(Roles = "Admin")]
        //public IActionResult ForgotPassword()
        //{
        //    string? getData = TempData["TransferData"] as string;

        //    // Use ViewBag or ViewData to pass data to the view
        //    ViewBag.PasswordResetAlert = getData;
        //    return View();
        //}

        //[HttpPost]

        ////[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _accountRepository.FindByEmailIdAsync(model.Email);

        //        if (user == null)
        //        {
        //            // Handle user not found
        //            TempData["TransferData"] = "Email does not exist.";
        //            return RedirectToAction("ForgotPassword");
        //        }

        //        var passwordUpdateResult = await _accountRepository.UpdatePasswordAsync(user, model.ConfirmPassword);


        //        if (passwordUpdateResult)
        //            TempData["TransferData"] = "Password updated successfully!!";
        //        else
        //            TempData["TransferData"] = "Password not updated!!";

        //        return RedirectToAction("ForgotPassword");

        //    }
        //    else
        //    {
        //        return View(model);
        //    }
        //}


        public static string GeneratePassword()
        {
            const int requiredLength = 8;
            const int requiredUniqueChars = 1;

            string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string lower = "abcdefghijklmnopqrstuvwxyz";
            string digits = "0123456789";
            string special = "!@#$%^&*()-_=+[]{}|;:<>?";

            Random rnd = new Random();

            // Ensure at least one of each required type
            List<char> passwordChars = new List<char>
            {
                upper[rnd.Next(upper.Length)],
                lower[rnd.Next(lower.Length)],
                digits[rnd.Next(digits.Length)],
                special[rnd.Next(special.Length)]
            };

            // Fill remaining characters (if required length > 4)
            string allChars = upper + lower + digits + special;

            while (passwordChars.Count < requiredLength)
            {
                passwordChars.Add(allChars[rnd.Next(allChars.Length)]);
            }

            // Shuffle to randomize positions
            passwordChars = passwordChars.OrderBy(x => rnd.Next()).ToList();

            // Ensure required unique chars condition is met
            if (passwordChars.Distinct().Count() < requiredUniqueChars)
            {
                // Replace last char with a special to introduce uniqueness
                char newChar;
                do
                {
                    newChar = allChars[rnd.Next(allChars.Length)];
                }
                while (passwordChars.Contains(newChar));

                passwordChars[^1] = newChar;
            }

            return new string(passwordChars.ToArray());
        }



        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _accountRepository.FindByEmailIdAsync(model.Email);

            if (user == null)
            {
                // DO NOT reveal user does not exist (security)
                TempData["TransferData"] = "User does not exist.";
                TempData["Status"] = "error";   // <--- important
                return RedirectToAction("ForgotPassword");
            }

            string password = GeneratePassword();

            await _accountRepository.UpdatePasswordAsync(user, password);

            string body = $@"
                           <p>Please log in using the password provided below,<br>
                           and make sure to change your password immediately after login.</p>
                           
                           <p><b>Your new password: {password}</b></p>
                           ";

            // SEND EMAIL
            await _emailService.SendEmailAsync(
             user.Email,
             "Reset Password",
             body
                );
            TempData["Status"] = "success";  // <--- important
            TempData["TransferData"] = "New password has been sent to your email.";
            return RedirectToAction("ForgotPassword");
        }


        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (email == null || token == null)
                return BadRequest("Invalid password reset token.");

            return View(new ResetPasswordModel { Email = email, Token = token });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _accountRepository.FindByEmailIdAsync(model.Email);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("ResetPassword");
            }

            var result = await _accountRepository.ResetPassword(user, model.Token, model.Password);

            if (result==true)
            {
                TempData["Success"] = "Password has been reset successfully.";
                return RedirectToAction("Login");
            }
            else
            {
                new ArgumentNullException(nameof(user), "result cannot be null.");
            }

                return View(model);

        }




    }
}
