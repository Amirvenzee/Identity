using identityPractice.ViewModels;
using IdentityPractice.Models;
using IdentityPractice.Repositories;
using IdentityPractice.Services;
using IdentityPractice.Srvices;
using IdentityPractice.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityPractice.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManger;
        private readonly SignInManager<ApplicationUser> _SignInManger;
          private readonly IMessageSender  _messageSender;
          private readonly ISmsService  _smsService;



        public LoginController(UserManager<ApplicationUser> userManger, SignInManager<ApplicationUser> signInManger, IMessageSender messageSender, ISmsService smsService)
        {
            _userManger = userManger;
            _SignInManger = signInManger;
            _messageSender = messageSender;
            _smsService = smsService;
        }




    


        [HttpGet]
       public IActionResult Register()
       {
            return View();
       }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                var newUser = new ApplicationUser()
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    EmailConfirmed = true,
                    PhoneNumber = model.PhoneNUmber,
                    PhoneNumberConfirmed = true,
                    
                    
                };

                var result = await _userManger.CreateAsync(newUser, model.Password);

                if (result.Succeeded)
                {
                    //var emailConfirmationToken =
                    //    await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var emailMessage = "ثبت نام انجام شد";
                    ////    Url.Action("ConfirmEmail", "Account",
                    ////        new { username = user.UserName, token = emailConfirmationToken },
                    ////        Request.Scheme);
                    //await _messageSender.SendEmailAsync(model.Email, "Aizen", emailMessage);
                    

                    return RedirectToAction(nameof(Login));
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }

            }

            return View(model);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsUserNameInUse(string userName)
        {
            var user = await _userManger.FindByNameAsync(userName);
            if (user == null) return Json(true);
            return Json("نام کاربری تکراری است ");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManger.FindByEmailAsync(email);
            if (user == null) return Json(true);
            return Json("نام کاربری تکراری است ");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
           await _SignInManger.SignOutAsync();
            return RedirectToAction("index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl=null)
        {
            if (_SignInManger.IsSignedIn(User)) return RedirectToAction("index", "Home");

            var model = new LoginViewModel()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _SignInManger.GetExternalAuthenticationSchemesAsync()).ToList()

            };

            ViewData["returnUrl"] = returnUrl;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (_SignInManger.IsSignedIn(User)) return RedirectToAction("index", "Home");

            model.ReturnUrl = returnUrl;
            model.ExternalLogins = (await _SignInManger.GetExternalAuthenticationSchemesAsync()).ToList();

            ViewData["returnUrl"] = returnUrl;



            if (ModelState.IsValid)
            {
                var user = await _userManger.FindByNameAsync(model.UserName);

                if(user == null)
                {
                    ModelState.AddModelError("", "نام کاربری یا رمز عبور اشتباه است");
                    return View(model);
                }

                var result = await _SignInManger.CheckPasswordSignInAsync(user,model.Password,true);



                if(result.Succeeded)
                {
                    

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        await _SignInManger.SignInAsync(user, model.RememberMe);
                        return Redirect(returnUrl);
                    }
                    await _SignInManger.SignInAsync(user, model.RememberMe);


                    return RedirectToAction("index", "Home");
                }
                if (result.IsLockedOut)
                {
                    ViewData["ErrorMessage"] = "اکانت شما به دلیل پنج بار ورود ناموفق به مدت پنج دقیقه قفل شده است";
                    return View(model);
                }

                ModelState.AddModelError("", "نام کاربری یا رمز عبور اشتباه است");
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallBack", "Login",
                new { ReturnUrl = returnUrl });

            var properties = _SignInManger.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallBack(string returnUrl = null, string remoteError = null)
        {
            ViewData["Title"] = returnUrl;
            returnUrl = (returnUrl != null && Url.IsLocalUrl(returnUrl)) ? returnUrl : Url.Content("~/");


            var loginViewModel = new LoginViewModel()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _SignInManger.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError("", $"Error : {remoteError}");
                return View("Login", loginViewModel);
            }

            var externalLoginInfo = await _SignInManger.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                ModelState.AddModelError("ErrorLoadingExternalLoginInfo", $"مشکلی پیش آمد");
                return View("Login", loginViewModel);
            }

            var signInResult = await _SignInManger.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider,
                externalLoginInfo.ProviderKey, false, true);

            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl);
            }

            var email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);

            if (email != null)
            {
                #region ورود مستقیم با گوگل
                //var user = await _userManager.FindByEmailAsync(email);
                //if (user == null)
                //{
                //    var userName = email.Split('@')[0];
                //    user = new ApplicationUser()
                //    {
                //        UserName = (userName.Length <= 10 ? userName : userName.Substring(0, 10)),
                //        Email = email,
                //        EmailConfirmed = true,
                //        City = "Tehran"

                //    };

                //    await _userManager.CreateAsync(user);
                //}

                //await _userManager.AddLoginAsync(user, externalLoginInfo);
                //await _signInManager.SignInAsync(user, false);

                //return Redirect(returnUrl);
                #endregion

                var user = await _userManger.FindByEmailAsync(email);
                if (user == null) return View();

                await _userManger.AddLoginAsync(user, externalLoginInfo);
                await _SignInManger.SignInAsync(user, true);

                return Redirect(returnUrl);
            }


            ViewData["ErrorMessage"] = $"دریافت کرد {externalLoginInfo.LoginProvider} نمیتوان اطلاعاتی از";
            return View("Login", loginViewModel);
        }

        [HttpPost]  //// این برای زمانی است که کاربر در وبسایت حساب نداشته است
        /////  اگه میخوای از کد های **** ورود مستقیم با گوگل***** استفاده کنی باید این متد رو کامنت کنی
        public async Task<IActionResult> ExternalLoginCallBack(ExternalLoginCallBackViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var loginViewModel = new LoginViewModel()
                {
                    ReturnUrl = returnUrl,
                    ExternalLogins = (await _SignInManger.GetExternalAuthenticationSchemesAsync()).ToList()
                };

                var externalLoginInfo = await _SignInManger.GetExternalLoginInfoAsync();
                if (externalLoginInfo?.Principal.FindFirstValue(ClaimTypes.Email) == null)
                {
                    ModelState.AddModelError("ErrorLoadingExternalLoginInfo", $"مشکلی پیش آمد");
                    return View("Login", loginViewModel);
                }

                var email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);

                var user = await _userManger.FindByEmailAsync(email);
                var result = new IdentityResult();
                if (user == null)
                {
                    user = new ApplicationUser()
                    {
                        Email = email,
                        UserName = model.UserName,
                        EmailConfirmed = true
                    };

                    if (!string.IsNullOrWhiteSpace(model.Password))
                        result = await _userManger.CreateAsync(user, model.Password);
                    else
                        result = await _userManger.CreateAsync(user);
                }

                if (result.Succeeded)
                {
                    await _userManger.AddLoginAsync(user, externalLoginInfo);
                    await _SignInManger.SignInAsync(user, false);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }



        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loginModel = new LoginViewModel 
                {
                    ExternalLogins = (await _SignInManger.GetExternalAuthenticationSchemesAsync()).ToList() 
                };

                var user = await _userManger.FindByEmailAsync(model.Email);

                if (user == null)
                    return View("Login", loginModel);
                
                    var token = await _userManger.GeneratePasswordResetTokenAsync(user);

                    var Link = Url.Action("ResetPassword", "Login", new { email = model.Email, token = token }, Request.Scheme);

                    await _messageSender.SendEmailAsync(model.Email, "Change Password", $"برای تغییر رمز عبور روی لیک زیر کلیک کن \n {Link}");

                    return RedirectToAction("Index", "Home");

               
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
                return RedirectToAction("index", "Home");
      


            var model = new ResetPasswordViewModel {  Email = email, Token = token  };
            return View(model);
        }

        [HttpPost]
         public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var login = new LoginViewModel
                {
                    ExternalLogins = (await _SignInManger.GetExternalAuthenticationSchemesAsync()).ToList()
                };

                var user = await _userManger.FindByEmailAsync(model.Email);

                if (user == null)
                    return View("login",login);

                var result = await _userManger.ResetPasswordAsync(user,model.Token,model.NewPassword);

                if (result.Succeeded)
                {
                    TempData["Success"] = "رمزعبور شما با موفقیت تغییر یافت";
                    return RedirectToAction("Login", "Login");
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }




            }

            return View(model);
        }

        


        /// <summary>
        /// ارسال کد تایید با پنل پیامکی
        /// </summary>
        /// <returns></returns>
        public IActionResult PhoneNumber()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> PhoneNumber(SetPhoneNumberViewModel phoneNumberDro)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManger.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumberDro.PhoneNumber);
                if (user == null)
                {
                    TempData["Error"] = "کابری با این شماره یافت نشد";
                    return View(phoneNumberDro);
                }
                var name = user.UserName;
                var code = Generator.RandomNumber();
                var message = $"ارسال کد برای {name} عزیز کد تایید : {code}";

                await _smsService.SendAsync(phoneNumberDro.PhoneNumber, message);

                ///Or This
                //await _smsService.SendLookupSMS(phoneNumberDro.PhoneNumber, "VerifyCode", user.UserName, code);
                TempData["PhoneNumber"] = phoneNumberDro.PhoneNumber;
                TempData["totp"] = code;
                return RedirectToAction(nameof(VerifyPhoneNumber));
            }

            return View(phoneNumberDro);
        }



        public IActionResult VerifyPhoneNumber()
        {


            return View(new VerifyPhoneNumberViewModel
            {
                PhoneNumber = TempData["PhoneNumber"].ToString(),
                Totp = TempData["totp"].ToString()
            });

        }


        [HttpPost]
        public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel verify)
        {
            var user = _userManger.Users.FirstOrDefault(x => x.PhoneNumber == verify.PhoneNumber);
            bool resultVerify = _smsService.VerifyTotp(verify.Code, verify.Totp);
            if (resultVerify == false)
            {
                ViewData["Message"] = $"کد وارد شده برای شماره {verify.PhoneNumber} اشتباه است";
                return View(verify);
            }
            else
            {

                user.PhoneNumberConfirmed = true;
                await _SignInManger.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }


        }

       

        [Authorize]
        public async Task<IActionResult> MyInfo()
        {
            var user = await _userManger.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var Model = new MyInfoViewModel
            {
                id = user.Id.ToString(),
                Email = user.Email,
              
                UserName = user.UserName,
                TwoFactor = user.TwoFactorEnabled,
              

            };
            return View(Model);
        }


        [HttpGet]
        public IActionResult TwoFactor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TwoFactor(TwoFactorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManger.GetUserAsync(User);



                var TwoFactorToken = await _userManger.GenerateTwoFactorTokenAsync(user, "Email");

                await _messageSender.SendEmailAsync(user.Email, "کد احراز هویت", $"کد احراز هویت شما : {TwoFactorToken}");


                TempData["HomePhoneNumber"] = model.HomePhoneNumber.ToString();

                return RedirectToAction("VerifyTwoFactor", "Login");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult VerifyTwoFactor()
        {
            var model = new VerifyTwoFactorViewModel
            {

                HomePhoneNumber = TempData["HomePhoneNumber"].ToString()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyTwoFactor(VerifyTwoFactorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManger.GetUserAsync(User);

                var result = await _userManger.VerifyTwoFactorTokenAsync(user, "Email", model.Code);
                if (result)
                {
                    user.HomePhoneNumber = model.HomePhoneNumber;
                    await _userManger.SetTwoFactorEnabledAsync(user, true);
                    await _SignInManger.RefreshSignInAsync(user);
                    return RedirectToAction("Index", "Home");
                }

                else
                {
                    ViewData["ErrorMessage"] = "کد اشتباه است ";
                    return View(model);
                }
            }
            return View(model);
        }


    }
}
