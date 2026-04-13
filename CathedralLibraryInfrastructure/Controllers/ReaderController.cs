using CathedralLibraryDomain.Model;
using CathedralLibraryInfrastructure.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibraryInfrastructure.Controllers
{
    public class ReaderController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly DbCathedralLibraryContext _context;

        public ReaderController(UserManager<User> userManager, DbCathedralLibraryContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = _userManager.GetUserId(User);
            var reader = await _context.Readers
                .Include(r => r.User)
                .Include(r => r.Requests)
                    .ThenInclude(req => req.Copy)
                        .ThenInclude(c => c.Publication)
                .FirstOrDefaultAsync(r => r.UserId == userId);

            if (reader == null)
            {
                return RedirectToAction("SetupProfile");
            }

            var model = new UserProfileViewModel
            {
                FullName = $"{reader.FirstName} {reader.LastName}",
                Email = reader.User?.Email ?? "",
                Faculty = reader.Faculty,
                Department = reader.Department,
                Position = reader.Position,
                Year = reader.User?.Year ?? 0,
                ActiveRequests = reader.Requests.Where(r => r.ReturnDate == null).ToList()
            };

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> SetupProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var reader = new Reader
            {
                UserId = user.Id,
                FirstName = user.First_name,
                LastName = user.Last_name,
                Faculty = "Не вказано",
                Department = "Не вказано",
                Position = "Студент"
            };

            _context.Readers.Add(reader);
            await _context.SaveChangesAsync();

            return RedirectToAction("Profile");
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit()
        {
            var userId = _userManager.GetUserId(User);
            var reader = await _context.Readers
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId == userId);

            if (reader == null) return NotFound();

            var model = new EditProfileViewModel
            {
                FirstName = reader.FirstName,
                LastName = reader.LastName,
                Faculty = reader.Faculty,
                Department = reader.Department,
                Position = reader.Position,
                Year = reader.User?.Year ?? 0
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = _userManager.GetUserId(User);
            var reader = await _context.Readers
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId == userId);

            if (reader == null) return NotFound();

            reader.FirstName = model.FirstName;
            reader.LastName = model.LastName;
            reader.Faculty = model.Faculty;
            reader.Department = model.Department;
            reader.Position = model.Position;

            if (reader.User != null)
            {
                reader.User.Year = model.Year;
                reader.User.First_name = model.FirstName;
                reader.User.Last_name = model.LastName;
            }

            _context.Readers.Update(reader);
            await _context.SaveChangesAsync();

            return RedirectToAction("Profile");
        }
    }
}