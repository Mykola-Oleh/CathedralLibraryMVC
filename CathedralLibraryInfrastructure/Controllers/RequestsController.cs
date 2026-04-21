using CathedralLibraryDomain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibraryInfrastructure.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class RequestsController : Controller
    {
        private readonly DbCathedralLibraryContext _context;
        private readonly UserManager<User> _userManager;

        public RequestsController(DbCathedralLibraryContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Requests/Create
        public async Task<IActionResult> Create(Guid publicationId, int copyNumber)
        {
            var copy = await _context.Copies
                .Include(c => c.Publication)
                .FirstOrDefaultAsync(c => c.PublicationId == publicationId && c.Copynumber == copyNumber);

            if (copy == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var reader = await _context.Readers.FirstOrDefaultAsync(r => r.UserId == userId);

            if (reader == null) return Challenge();

            var request = new Request
            {
                PublicationId = publicationId,
                Copynumber = copyNumber,
                ReaderId = reader.Id,
                RequestDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14),
                StatusId = 1
            };

            ViewBag.PublicationTitle = copy.Publication.Title;
            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Request request)
        {
            ModelState.Remove("Copy");
            ModelState.Remove("Reader");
            ModelState.Remove("Status");
            bool isBusy = await _context.Requests.AnyAsync(r =>
                r.PublicationId == request.PublicationId &&
                r.Copynumber == request.Copynumber &&
                r.ReturnDate == null);

            if (isBusy)
            {
                ModelState.AddModelError("", "Цей примірник зараз виданий іншому читачеві.");
                var copy = await _context.Copies.Include(c => c.Publication)
                    .FirstOrDefaultAsync(c => c.PublicationId == request.PublicationId && c.Copynumber == request.Copynumber);
                ViewBag.PublicationTitle = copy?.Publication.Title;
                return View(request);
            }
            if (ModelState.IsValid)
            {
                request.Id = Guid.NewGuid();

                var copy = await _context.Copies
                    .FirstOrDefaultAsync(c => c.PublicationId == request.PublicationId && c.Copynumber == request.Copynumber);

                if (copy != null)
                {
                    copy.StatusId = await GetOrCreateCopyStatusIdAsync("Заброньовано");
                    _context.Update(copy);
                }

                _context.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction("MyRequests");
            }
            return View(request);
        }
        public async Task<IActionResult> MyRequests()
        {
            var userId = _userManager.GetUserId(User);
            var requests = await _context.Requests
                .Include(r => r.Copy.Publication)
                .Include(r => r.Status)
                .Where(r => r.Reader.UserId == userId)
                .ToListAsync();
            return View(requests);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Approve(Guid id)
        {
            var request = await _context.Requests
                .Include(r => r.Copy)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null) return NotFound();
            request.StatusId = 2;

            if (request.Copy != null)
            {
                request.Copy.StatusId = await GetOrCreateCopyStatusIdAsync("Видано");
                _context.Update(request.Copy);
            }

            request.RequestDate = DateTime.SpecifyKind(request.RequestDate, DateTimeKind.Utc);
            request.DueDate = DateTime.SpecifyKind(request.DueDate, DateTimeKind.Utc);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AllRequests));
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Reject(Guid id)
        {
            var request = await _context.Requests.Include(r => r.Copy).FirstOrDefaultAsync(r => r.Id == id);
            if (request == null) return NotFound();

            request.StatusId = 3;

            if (request.Copy != null)
            {
                request.Copy.StatusId = await GetOrCreateCopyStatusIdAsync("В наявності");
                _context.Update(request.Copy);
            }

            request.RequestDate = DateTime.SpecifyKind(request.RequestDate, DateTimeKind.Utc);
            request.DueDate = DateTime.SpecifyKind(request.DueDate, DateTimeKind.Utc);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AllRequests));
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AllRequests()
        {
            var requests = await _context.Requests
                .Include(r => r.Copy.Publication)
                .Include(r => r.Status)
                .Include(r => r.Reader)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
            return View(requests);
        }

        private async Task<int> GetOrCreateCopyStatusIdAsync(string statusName)
        {
            var status = await _context.Copystatuses
                .FirstOrDefaultAsync(s => s.StatusName == statusName);

            if (status == null)
            {
                status = new Copystatus { StatusName = statusName };
                _context.Copystatuses.Add(status);
                await _context.SaveChangesAsync();
            }

            return status.Id;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestReturn(Guid id)
        {
            var userId = _userManager.GetUserId(User);
            var request = await _context.Requests
                .FirstOrDefaultAsync(r => r.Id == id && r.Reader.UserId == userId);

            if (request == null) return NotFound();
            request.StatusId = 4;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Заявку на повернення надіслано. Будь ласка, принесіть книгу до бібліотеки.";
            return RedirectToAction("MyRequests");
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmReturn(Guid id)
        {
            var request = await _context.Requests
                .Include(r => r.Copy)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null) return NotFound();

            request.ReturnDate = DateTime.UtcNow;
            request.StatusId = 5;

            if (request.Copy != null)
            {
                request.Copy.StatusId = await GetOrCreateCopyStatusIdAsync("В наявності");
                _context.Update(request.Copy);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Повернення книги підтверджено!";
            return RedirectToAction(nameof(AllRequests));
        }
    }
}
