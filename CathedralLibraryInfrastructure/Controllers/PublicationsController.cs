using CathedralLibraryDomain.Model;
using CathedralLibraryInfrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibraryInfrastructure.Controllers
{
    public class PublicationsController : Controller
    {
        private readonly DbCathedralLibraryContext _context;

        private readonly IDataPortServiceFactory<Publication> _factory;

        public PublicationsController(DbCathedralLibraryContext context, IDataPortServiceFactory<Publication> factory)
        {
            _context = context;
            _factory = factory;
        }

        // GET: Publications
        [Authorize]
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewBag.TitleSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.YearSortParm = sortOrder == "Year" ? "year_desc" : "Year";
            ViewBag.TypeSortParm = sortOrder == "Type" ? "type_desc" : "Type";
            ViewBag.CurrentSort = sortOrder;

            var publications = _context.Publications.Include(p => p.PublicationType).AsQueryable()
        .Include(p => p.PublicationType)
        .Include(p => p.Authors)
        .AsQueryable();
            switch (sortOrder)
            {
                case "name_desc":
                    publications = publications.OrderByDescending(s => s.Title);
                    break;
                case "Year":
                    publications = publications.OrderBy(s => s.Year);
                    break;
                case "year_desc":
                    publications = publications.OrderByDescending(s => s.Year);
                    break;
                case "Type":
                    publications = publications.OrderBy(s => s.PublicationType.Name);
                    break;
                case "type_desc":
                    publications = publications.OrderByDescending(s => s.PublicationType.Name);
                    break;
                default:
                    publications = publications.OrderBy(s => s.Title);
                    break;
            }

            return View(await publications.ToListAsync());
        }

        // GET: Publications/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var publication = await _context.Publications
                .Include(p => p.PublicationType)
                .Include(p => p.Authors)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (publication == null) return NotFound();

            return View(publication);
        }

        // Метод для генерації списку років
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

        // GET: Publications/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewBag.Years = GetYearsList(DateTime.Now.Year);
            ViewData["PublicationTypeId"] = new SelectList(_context.PublicationType, "Id", "Name");
            ViewBag.Authors = new MultiSelectList(_context.Authors, "Id", "LastName");
            return View();
        }
        // POST: Publications/Create
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Publication publication, Guid[] selectedAuthors)
        {
            if (ModelState.IsValid)
            {
                if (selectedAuthors != null)
                {
                    foreach (var authorId in selectedAuthors)
                    {
                        var author = await _context.Authors.FindAsync(authorId);
                        if (author != null) publication.Authors.Add(author);
                    }
                }
                publication.Id = Guid.NewGuid();
                _context.Add(publication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PublicationTypeId"] = new SelectList(_context.PublicationType, "Id", "Name");
            ViewBag.Years = GetYearsList(DateTime.Now.Year);
            return View(publication);
        }

        // GET: Publications/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var publication = await _context.Publications
                .Include(p => p.Authors)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (publication == null) return NotFound();

            ViewBag.PublicationTypeId = new SelectList(_context.PublicationType, "Id", "Name", publication.PublicationTypeId);
            var allAuthors = await _context.Authors.ToListAsync();
            ViewBag.Authors = allAuthors.Select(a => new SelectListItem
            {
                Text = $"{a.LastName} {a.FirstName}",
                Value = a.Id.ToString(),
                Selected = publication.Authors.Any(pa => pa.Id == a.Id)
            }).ToList();

            ViewBag.Years = GetYearsList(publication.Year);
            return View(publication);
        }

        // POST: Publications/Edit/5
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Annotation,PublicationTypeId,Year")] Publication publication, Guid[] selectedAuthors)
        {
            if (id != publication.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var pubToUpdate = await _context.Publications
                        .Include(p => p.Authors)
                        .FirstOrDefaultAsync(m => m.Id == id);
                    pubToUpdate.Title = publication.Title;
                    pubToUpdate.Annotation = publication.Annotation;
                    pubToUpdate.PublicationTypeId = publication.PublicationTypeId;
                    pubToUpdate.Year = publication.Year;
                    pubToUpdate.Authors.Clear();
                    if (selectedAuthors != null)
                    {
                        foreach (var authorId in selectedAuthors)
                        {
                            var author = await _context.Authors.FindAsync(authorId);
                            if (author != null) pubToUpdate.Authors.Add(author);
                        }
                    }

                    _context.Update(pubToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) { /* ... */ }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Years = GetYearsList(publication.Year);
            ViewData["PublicationTypeId"] = new SelectList(_context.PublicationType, "Id", "Name");
            return View(publication);
        }

        [Authorize]
        public async Task<IActionResult> Copies(Guid? id)
        {
            if (id == null) return NotFound();
            return RedirectToAction("Index", "Copies", new { id = id });
        }

        // GET: Publications/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publication = await _context.Publications
                .Include(p => p.PublicationType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (publication == null)
            {
                return NotFound();
            }

            return View(publication);
        }

        // POST: Publications/Delete/5
        [Authorize(Roles = "admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var publication = await _context.Publications.FindAsync(id);
            if (publication != null)
            {
                _context.Publications.Remove(publication);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PublicationExists(Guid id)
        {
            return _context.Publications.Any(e => e.Id == id);
        }
        [HttpGet]
        public async Task<IActionResult> Export(CancellationToken cancellationToken)
        {
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var exportService = _factory.GetExportService(contentType);
            var memoryStream = new MemoryStream();

            await exportService.WriteToAsync(memoryStream, cancellationToken);
            memoryStream.Position = 0;

            return File(memoryStream, contentType, $"publications_{DateTime.Now:yyyyMMdd}.xlsx");
        }
        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Import(IFormFile fileExcel, CancellationToken cancellationToken)
        {
            try
            {
                if (fileExcel != null && fileExcel.Length > 0)
                {
                    var importService = _factory.GetImportService(fileExcel.ContentType);
                    using (var stream = fileExcel.OpenReadStream())
                    {
                        await importService.ImportFromStreamAsync(stream, cancellationToken);
                    }
                    TempData["Success"] = "Дані успішно імпортовано!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Помилка при імпорті: перевірте формат файлу";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}