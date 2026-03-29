using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CathedralLibraryDomain.Model;
using CathedralLibraryInfrastructure;
using Microsoft.AspNetCore.Authorization;

namespace CathedralLibraryInfrastructure.Controllers
{
    public class PublicationsController : Controller
    {
        private readonly DbCathedralLibraryContext _context;

        public PublicationsController(DbCathedralLibraryContext context)
        {
            _context = context;
        }

        // GET: Publications
        [Authorize]
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewBag.TitleSortParm = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewBag.YearSortParm = sortOrder == "Year" ? "year_desc" : "Year";
            ViewBag.TypeSortParm = sortOrder == "Type" ? "type_desc" : "Type";

            var publications = _context.Publications
                .Include(p => p.PublicationType)
                .AsQueryable();
            switch (sortOrder)
            {
                case "title_desc":
                    publications = publications.OrderByDescending(p => p.Title);
                    break;
                case "Year":
                    publications = publications.OrderBy(p => p.Year);
                    break;
                case "year_desc":
                    publications = publications.OrderByDescending(p => p.Year);
                    break;
                case "Type":
                    publications = publications.OrderBy(p => p.PublicationTypeId);
                    break;
                case "type_desc":
                    publications = publications.OrderByDescending(p => p.PublicationTypeId);
                    break;
                default:
                    publications = publications.OrderBy(p => p.Title);
                    break;
            }

            return View(await publications.AsNoTracking().ToListAsync());
        }

        // GET: Publications/Details/5
        public async Task<IActionResult> Details(Guid? id)
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
            return View();
        }

        // POST: Publications/Create
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Annotation,PublicationTypeId,Year")] Publication publication)
        {
            if (ModelState.IsValid)
            {
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
            if (id == null)
            {
                return NotFound();
            }

            var publication = await _context.Publications.FindAsync(id);
            if (publication == null)
            {
                return NotFound();
            }
            ViewBag.Years = GetYearsList(publication.Year);
            ViewData["PublicationTypeId"] = new SelectList(_context.PublicationType, "Id", "Name");
            return View(publication);
        }

        // POST: Publications/Edit/5
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Annotation,PublicationTypeId,Year")] Publication publication)
        {
            if (id != publication.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(publication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublicationExists(publication.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
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
    }
}