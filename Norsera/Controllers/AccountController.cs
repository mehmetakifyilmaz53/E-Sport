using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Norsera.business.Abstract;
using Norsera.Emailservices;
using Norsera.Extensions;
using Norsera.Identity;
using Norsera.Models;

namespace Norsera.Controllers
{

    public class AccountController : Controller
    {

        private UserManager<User> _userManager;

        private SignInManager<User> _signInManager;

        private IEmailSender _emailSender;

        private ICartService _cartService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender,ICartService cartService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _cartService = cartService;
        }
       

        public IActionResult Login(string ReturnUrl = null)
        {
            return View(new LoginModel()
            {
                ReturnUrl = ReturnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // var user = await _userManager.FindByNameAsync(model.UserName);
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "An account has not been created with this username before.");
                return View(model);
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Please confirm by clicking the link sent to your email account.");
                return View(model);
            }



            var resutl = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (resutl.Succeeded)
            {
                return Redirect(model.ReturnUrl ?? "~/");
            }

            ModelState.AddModelError("", "The entered username or password is incorrect.");

            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email
            };

            var resutl = await _userManager.CreateAsync(user, model.Password);

            if (resutl.Succeeded)
            {
                // generate token

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail", "Account", new
                {
                    userId = user.Id,
                    token = code
                });
                Console.WriteLine(url);
                // email
                await _emailSender.SendEmailAsync(model.Email,
                "Please confirm your account",
                $"Please click the link to confirm your email account <a href='http://localhost:5137{url}'></a>");


                TempData.Put("message", new AlertMessage()
            {
                Title = "Registration Successful",
                Message = "Account creation successful. Please don't forget to confirm your email.",
                AlertType = "success"
            });

                return RedirectToAction("Login", "Account");
            }

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }

            // ModelState.AddModelError("", "Bilinmeyen hata oldu tekrar deneyiniz.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            TempData.Put("message", new AlertMessage()
            {
                Title = "Session Closed",
                Message = "Your account has been securely closed.",
                AlertType = "warning"
            });
            await _signInManager.SignOutAsync();
            return Redirect("~/");
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Invalid Token",
                    Message = "Invalid token.",
                    AlertType = "danger"
                });

          
                return View();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var resutl = await _userManager.ConfirmEmailAsync(user, token);
                if (resutl.Succeeded)
                {
                    // cart objesini oluştur.
                    _cartService.InitializeCart(user.Id);
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "Your Account is Verified",
                        Message = "Your account has been verified.",
                        AlertType = "success"
                    });
                    return View();
                }
            }

            TempData.Put("message", new AlertMessage()
            {
                Title = "Your Account is Not Verified",
                Message = "Your account has not been verified.",
                AlertType = "warning"
            });

            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {

                TempData.Put("message", new AlertMessage()
                {
                    Title = "Email Required",
                    Message = "You need to enter an email address.",
                    AlertType = "danger"
                });
                return View();
            }
            var user = await _userManager.FindByEmailAsync(Email);

            if (user == null)
            {

                TempData.Put("message", new AlertMessage()
                {
                    Title = "Email Account Not Found",
                    Message = "No email account found with this email address.",
                    AlertType = "danger"
                });
                return View();
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var url = Url.Action("ResetPassword", "Account", new
            {
                userId = user.Id,
                token = code
            });
            Console.WriteLine(url);
            // email
            await _emailSender.SendEmailAsync(Email,
            "Reset Paswword",
            $"To reset your password, please <a href='http://localhost:5137{url}'></a>");


            TempData.Put("message", new AlertMessage()
            {
                Title = "Password Reset Sent",
                Message = "Password Reset Sent",
                AlertType = "success"
            });
            return View();
        }

        public IActionResult ResetPassword(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Shop", "Index");
            }

            var model = new ResetPasswordModel
            {
                Token = token
            };

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {

                TempData.Put("message", new AlertMessage()
                {
                    Title = "Password Field Cannot Be Empty",
                    Message = "Password field cannot be empty.",
                    AlertType = "danger"
                });

                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                  TempData.Put("message", new AlertMessage()
            {
                Title = "An Error Occurred",
                Message = "An error occurred.",
                AlertType = "danger"
            });
                return RedirectToAction("Shop", "Index");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }


        // private void CreateMessage(string message, string alerttype) tempdata ekledğimiz için bunu siliyopruız

        // {
        //     var msg = new AlertMessage()
        //     {
        //         Message = message,
        //         AlertType = alerttype
        //     };
        //     TempData["message"] = JsonConvert.SerializeObject(msg);
        // }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}