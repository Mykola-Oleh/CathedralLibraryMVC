using CathedralLibraryDomain.Model;
using CathedralLibraryInfrastructure.Models;
using CathedralLibraryInfrastructure.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CathedralLibraryInfrastructure.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Years = GetYearsList(DateTime.Now.Year);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User {First_name=model.First_name,Last_name=model.Last_name, Email = model.Email, UserName = model.Email, Year = model.Year }; ViewBag.Years = GetYearsList(DateTime.Now.Year);

                var result = await _userManager.CreateAsync(user, model.Password);

                /*if (result.Succeeded)
                {
                    return Content("Користувач успішно створений в базі PostgreSQL!");
                }*/
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
        // ВХІД (АВТОРИЗАЦІЯ)
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        // Якщо ReturnUrl порожній, ОБОВ'ЯЗКОВО повертаємо на головну
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильний логін чи (та) пароль");
                }
            }
            return View(model);
        }

        // ВИХІД
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private IEnumerable<SelectListItem> GetYearsList(int? selectedYear = null)
        {
            int currentYear = DateTime.Now.Year;
            var years = Enumerable.Range(1900, currentYear - 1900 + 1)
                                  .OrderByDescending(y => y)
                                  .Select(y => new SelectListItem
                                  {
                                      Value = y.ToString(),
                                      Text = y.ToString(),
                                      Selected = (selectedYear.HasValue && y == selectedYear.Value)
                                  });
            return years;
        }
    }
}