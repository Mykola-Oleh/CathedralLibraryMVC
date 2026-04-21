using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibraryInfrastructure.Controllers
{
    [Authorize(Roles = "admin")]
    public class FundchangesController : Controller
    {
        private readonly DbCathedralLibraryContext _context;

        public FundchangesController(DbCathedralLibraryContext context)
        {
            _context = context;
        }

        // GET: Fundchanges
        public async Task<IActionResult> Index()
        {
            var changes = await _context.Fundchanges
                .Include(f => f.Publication)
                .OrderByDescending(f => f.ChangeDate)
                .ToListAsync();

            return View(changes);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "super admin")] 
        public async Task<IActionResult> Delete(Guid id)
        {
            var change = await _context.Fundchanges.FindAsync(id);
            if (change == null)
            {
                return NotFound();
            }

            _context.Fundchanges.Remove(change);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Запис успішно видалено з журналу.";
            return RedirectToAction(nameof(Index));
        }
    }
}
