using DocumentGenerationApplication.Models.UserModel;
using DocumentGenerationApplication.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DocumentGenerationApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;


        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }


        [Route("Signup")]
        //[Authorize(Roles = "Admin")]
        public IActionResult SignUp()
        {
            return View();
        }

        //[HttpPost]
        //[Route("Signup")]
        ////[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> SignUp(SignUpUserModel UserModel)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        var result = await _accountRepository.CreateUserAsync(UserModel);

        //        if (!result.Succeeded)
        //        {
        //            foreach (var errorMessage in result.Errors)
        //            {
        //                if (errorMessage.Code == "DuplicateUserName")
        //                {

        //                    ModelState.AddModelError("", "Employee-Code is already taken.");
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError("", errorMessage.Description);
        //                }
        //            }

        //        }
        //        else
        //        {
        //            ModelState.Clear();

        //            ViewBag.SignUpAlert = "SignUp Successful!!";
        //        }
        //    }
        //    return View();
        //}

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
                    return RedirectToAction("GetAllOfferLetters", "Employee");

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



        //[Authorize(Roles = "Admin")]
        public IActionResult ForgotPassword()
        {
            string? getData = TempData["TransferData"] as string;

            // Use ViewBag or ViewData to pass data to the view
            ViewBag.PasswordResetAlert = getData;
            return View();
        }

        [HttpPost]

        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _accountRepository.FindByEmailIdAsync(model.Email);

                if (user == null)
                {
                    // Handle user not found
                    TempData["TransferData"] = "Email does not exist.";
                    return RedirectToAction("ForgotPassword");
                }

                var passwordUpdateResult = await _accountRepository.UpdatePasswordAsync(user, model.ConfirmPassword);


                if (passwordUpdateResult)
                    TempData["TransferData"] = "Password updated successfully!!";
                else
                    TempData["TransferData"] = "Password not updated!!";

                return RedirectToAction("ForgotPassword");

            }
            else
            {
                return View(model);
            }
        }
    }
}
