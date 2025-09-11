using BlockVotes.Data;
using BlockVotes.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlockVotes.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register() => View();
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(string firstName,string lastName,string email,string password)
        {
            var user = new AppUser { FirstName = firstName, LastName = lastName, Email = email,UserName=email };
            user.EmailConfirmed = true;
            var result = await userManager.CreateAsync(user,password);
            if (result.Succeeded)
            {
                // Sign in
                await signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Voting");
            }
            ViewBag.Error = string.Join(" - ", result.Errors.Select(e=>e.Description));
            return View();
        }
        [HttpGet]
        public IActionResult Login() => View();
        [HttpPost]
        public async Task<IActionResult> Login(string email,string password)
        {
            //var user = await userManager.FindByEmailAsync(email);
            //if(user != null)
            //{
                var result = await signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Voting");
                }else if (!result.Succeeded)
                {
                    if (result.IsLockedOut) ViewBag.Error = "Account is locked out";
                    else if (result.IsNotAllowed) ViewBag.Error = "Login not allowed";
                    else ViewBag.Error = "Wrong credentials";
                }
            //}
            //else
            //{
            //    ViewBag.Error = "Wrong credentials";
            //}

            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
