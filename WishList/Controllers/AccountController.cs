using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WishList.Models;
using WishList.Models.AccountViewModels;

namespace WishList.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Register(RegisterViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", viewModel);
            }

            var applicationUser = new ApplicationUser() {Email = viewModel.Email, UserName = viewModel.Email};

            var result = _userManager.CreateAsync(applicationUser, viewModel.Password).Result;

            if (!result.Succeeded)
            {
                foreach (var identityError in result.Errors)
                {
                    ModelState.AddModelError("Password", identityError.Description);
                }

                return View("Register", viewModel);
            }

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", viewModel);
            }

            var result = _signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, false, false).Result;

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return RedirectToAction("Login", ModelState);
            }
            
            return RedirectToAction("Index", "Item");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
