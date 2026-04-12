using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CathedralLibraryDomain.Model;
using CathedralLibraryInfrastructure.ViewModels;

namespace CathedralLibraryInfrastructure.Controllers
{
    public class AdminToolController : Controller
    {
        private readonly DbCathedralLibraryContext _context;

        public AdminToolController(DbCathedralLibraryContext context)
        {
            _context = context;
        }

        // Головна сторінка з вкладками
        public async Task<IActionResult> Index(string tab = "PublicationType")
        {
            var model = new AdminToolViewModel
            {
                PublicationTypes = await _context.PublicationType.ToListAsync(),
                RequestStatuses = await _context.Requeststatuses.ToListAsync(),
                CopyStatuses = await _context.Copystatuses.ToListAsync(),
                ActiveTab = tab
            };
            return View(model);
        }

        // Приклад методу редагування для PublicationType
        [HttpGet]
        public async Task<IActionResult> EditPublicationType(int id)
        {
            var item = await _context.PublicationType.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> EditPublicationType(PublicationType item)
        {
            if (ModelState.IsValid)
            {
                _context.Update(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { tab = "PublicationType" });
            }
            return View(item);
        }

        // --- Методи для Requeststatus ---
        [HttpGet]
        public async Task<IActionResult> EditRequestStatus(int id)
        {
            var item = await _context.Requeststatuses.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRequestStatus(Requeststatus item)
        {
            if (ModelState.IsValid)
            {
                _context.Update(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { tab = "RequestStatus" });
            }
            return View(item);
        }

        // --- Методи для Copystatus ---
        [HttpGet]
        public async Task<IActionResult> EditCopyStatus(int id)
        {
            var item = await _context.Copystatuses.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCopyStatus(Copystatus item)
        {
            if (ModelState.IsValid)
            {
                _context.Update(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { tab = "CopyStatus" });
            }
            return View(item);
        }

        // --- Create Методи ---

        [HttpGet]
        public IActionResult CreatePublicationType() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePublicationType(PublicationType item)
        {
            if (ModelState.IsValid)
            {
                _context.PublicationType.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { tab = "PublicationType" });
            }
            return View(item);
        }

        [HttpGet]
        public IActionResult CreateRequestStatus() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRequestStatus(Requeststatus item)
        {
            if (ModelState.IsValid)
            {
                _context.Requeststatuses.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { tab = "RequestStatus" });
            }
            return View(item);
        }

        [HttpGet]
        public IActionResult CreateCopyStatus() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCopyStatus(Copystatus item)
        {
            if (ModelState.IsValid)
            {
                _context.Copystatuses.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { tab = "CopyStatus" });
            }
            return View(item);
        }

        // --- Видалення ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string type)
        {
            if (type == "PublicationType")
            {
                var defaultTypeName = "На уточненні";
                var defaultType = await _context.PublicationType
                    .FirstOrDefaultAsync(t => t.Name == defaultTypeName);

                if (defaultType == null)
                {
                    defaultType = new PublicationType { Name = defaultTypeName };
                    _context.PublicationType.Add(defaultType);
                    await _context.SaveChangesAsync();
                }

                if (id == defaultType.Id)
                {
                    TempData["ErrorMessage"] = "Неможливо видалити системний тип 'На уточненні'.";
                    return RedirectToAction(nameof(Index), new { tab = type });
                }

                var publicationsToUpdate = _context.Publications.Where(p => p.PublicationTypeId == id);

                foreach (var pub in publicationsToUpdate)
                {
                    pub.PublicationTypeId = defaultType.Id;
                }

                var itemToDelete = await _context.PublicationType.FindAsync(id);
                if (itemToDelete != null)
                {
                    _context.PublicationType.Remove(itemToDelete);
                }
            }
            else if (type == "CopyStatus")
            {
                var defaultName = "На уточненні";

                var defaultStatus = await _context.Copystatuses
                    .FirstOrDefaultAsync(s => s.StatusName == defaultName);

                if (defaultStatus == null)
                {
                    defaultStatus = new Copystatus { StatusName = defaultName };
                    _context.Copystatuses.Add(defaultStatus);
                    await _context.SaveChangesAsync();
                }

                if (id == defaultStatus.Id)
                {
                    TempData["ErrorMessage"] = "Неможливо видалити системний статус 'На уточненні'.";
                    return RedirectToAction(nameof(Index), new { tab = type });
                }

                var copiesToUpdate = _context.Copies.Where(c => c.StatusId == id);
                foreach (var copy in copiesToUpdate)
                {
                    copy.StatusId = defaultStatus.Id;
                }

                var itemToDelete = await _context.Copystatuses.FindAsync(id);
                if (itemToDelete != null) _context.Copystatuses.Remove(itemToDelete);
            }
            else if (type == "RequestStatus")
            {
                var defaultName = "Очікує обробки";

                var defaultStatus = await _context.Requeststatuses
                    .FirstOrDefaultAsync(s => s.StatusName == defaultName);

                if (defaultStatus == null)
                {
                    defaultStatus = new Requeststatus { StatusName = defaultName };
                    _context.Requeststatuses.Add(defaultStatus);
                    await _context.SaveChangesAsync();
                }

                if (id == defaultStatus.Id)
                {
                    TempData["ErrorMessage"] = $"Неможливо видалити системний статус '{defaultName}'.";
                    return RedirectToAction(nameof(Index), new { tab = type });
                }
                var requestsToUpdate = _context.Requests.Where(r => r.StatusId == id);
                foreach (var req in requestsToUpdate)
                {
                    req.StatusId = defaultStatus.Id;
                }

                var itemToDelete = await _context.Requeststatuses.FindAsync(id);
                if (itemToDelete != null) _context.Requeststatuses.Remove(itemToDelete);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = type });
        }

    }
}