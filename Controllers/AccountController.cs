using BookStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<DefaultUser> _signInManager;

        public AccountController(SignInManager<DefaultUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public IActionResult Blocked()
        {
            return View("~/Views/Shared/Blocked.cshtml"); 
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home"); 
        }
    }
}
