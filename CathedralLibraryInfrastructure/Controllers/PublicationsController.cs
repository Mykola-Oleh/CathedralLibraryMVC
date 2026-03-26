using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CathedralLibraryDomain.Model;
using CathedralLibraryInfrastructure;

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
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewBag.TitleSortParm = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewBag.YearSortParm = sortOrder == "Year" ? "year_desc" : "Year";
            ViewBag.TypeSortParm = sortOrder == "Type" ? "type_desc" : "Type";

            var publications = from p in _context.Publications
                               select p;
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
                    publications = publications.OrderBy(p => p.PublicationType);
                    break;
                case "type_desc":
                    publications = publications.OrderByDescending(p => p.PublicationType);
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
        public IActionResult Create()
        {
            ViewBag.Years = GetYearsList(DateTime.Now.Year);
            return View();
        }

        // POST: Publications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Annotation,PublicationType,Year")] Publication publication)
        {
            if (ModelState.IsValid)
            {
                publication.Id = Guid.NewGuid();
                _context.Add(publication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Years = GetYearsList(DateTime.Now.Year);
            return View(publication);
        }

        // GET: Publications/Edit/5
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
            return View(publication);
        }

        // POST: Publications/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Annotation,PublicationType,Year")] Publication publication)
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
            return View(publication);
        }

        public async Task<IActionResult> Copies(Guid? id)
        {
            if (id == null) return NotFound();
            return RedirectToAction("Index", "Copies", new { id = id });
        }

        // GET: Publications/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publication = await _context.Publications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (publication == null)
            {
                return NotFound();
            }

            return View(publication);
        }

        // POST: Publications/Delete/5
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