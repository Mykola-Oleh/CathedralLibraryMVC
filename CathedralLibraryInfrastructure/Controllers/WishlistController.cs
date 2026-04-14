using CathedralLibraryDomain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibraryInfrastructure.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly DbCathedralLibraryContext _context;
        private readonly UserManager<User> _userManager;

        public WishlistController(DbCathedralLibraryContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null) return Challenge();

            var wishlistItems = await _context.Wishlists
                .Include(w => w.Publication)
                    .ThenInclude(p => p.PublicationType)
                .Include(w => w.Publication)
                    .ThenInclude(p => p.Authors)
                .Where(w => w.Reader.UserId == userId)
                .ToListAsync();

            return View(wishlistItems);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Guid publicationId)
        {
            var userId = _userManager.GetUserId(User);
            var reader = await _context.Readers
                .FirstOrDefaultAsync(r => r.UserId == userId);

            if (reader == null) return BadRequest();
            var exists = await _context.Wishlists
                .AnyAsync(w => w.ReaderId == reader.Id && w.PublicationId == publicationId);

            if (!exists)
            {
                var wish = new Wishlist
                {
                    ReaderId = reader.Id,
                    PublicationId = publicationId
                };
                _context.Wishlists.Add(wish);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Remove(Guid id)
        {
            var wish = await _context.Wishlists.FindAsync(id);
            if (wish != null)
            {
                _context.Wishlists.Remove(wish);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> ToggleWishlist(Guid publicationId)
        {
            var userId = _userManager.GetUserId(User);
            var reader = await _context.Readers.FirstOrDefaultAsync(r => r.UserId == userId);

            if (reader == null) return Unauthorized();

            var existing = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.ReaderId == reader.Id && w.PublicationId == publicationId);

            if (existing != null)
            {
                _context.Wishlists.Remove(existing);
                await _context.SaveChangesAsync();
                return Json(new { success = true, action = "removed" });
            }
            else
            {
                var wish = new Wishlist { ReaderId = reader.Id, PublicationId = publicationId };
                _context.Wishlists.Add(wish);
                await _context.SaveChangesAsync();
                return Json(new { success = true, action = "added" });
            }
        }
    }
}
