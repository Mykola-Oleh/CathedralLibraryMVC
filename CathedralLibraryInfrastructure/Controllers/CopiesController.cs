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
    public class CopiesController : Controller
    {
        private readonly DbCathedralLibraryContext _context;

        public CopiesController(DbCathedralLibraryContext context)
        {
            _context = context;
        }
        private bool CopyExists(Guid id, int copyNum)
        {
            return _context.Copies.Any(e => e.PublicationId == id && e.Copynumber == copyNum);
        }
        // GET: Copies
        public async Task<IActionResult> Index(Guid? id)
        {
            if(id == null) return RedirectToAction("Index", "Publications");

            ViewBag.PublicationId = id;
            var copiesByPublication = _context.Copies.Where(c => c.PublicationId == id).Include(c => c.Publication).Include(c => c.Status);
            return View(await copiesByPublication.ToListAsync());
        }

        // GET: Copies/Details/5
        public async Task<IActionResult> Details(Guid? id, int? copyNum)
        {
            if (id == null || copyNum == null)
            {
                return NotFound();
            }

            var copy = await _context.Copies
                .Include(c => c.Publication)
                .Include(c => c.Status)
                .FirstOrDefaultAsync(m => m.PublicationId == id && m.Copynumber == copyNum);

            if (copy == null)
            {
                return NotFound();
            }

            return View(copy);
        }

        // GET: Copies/Create
        public IActionResult Create(Guid? id) 
        {
            ViewBag.PublicationId = id;
            ViewData["PublicationId"] = new SelectList(_context.Publications, "Id", "Title", id);
            ViewData["StatusId"] = new SelectList(_context.Copystatuses, "Id", "StatusName");
            return View();
        }


        // POST: Copies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PublicationId,Copynumber,StatusId")] Copy copy)
        {
            ModelState.Remove("Publication");
            ModelState.Remove("Status");
            if (ModelState.IsValid)
            {
                _context.Add(copy);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index), new { id = copy.PublicationId });
            }
            ViewData["PublicationId"] = new SelectList(_context.Publications, "Id", "Title", copy.PublicationId);
            ViewData["StatusId"] = new SelectList(_context.Copystatuses, "Id", "StatusName", copy.StatusId);
            return View(copy);
        }

        // GET: Copies/Edit/5
        public async Task<IActionResult> Edit(Guid? id, int? copyNum)
        {
            if (id == null)
            {
                return NotFound();
            }

            var copy = await _context.Copies.FindAsync(id, copyNum);
            if (copy == null)
            {
                return NotFound();
            }
            ViewBag.PublicationId = id;
            ViewData["PublicationId"] = new SelectList(_context.Publications, "Id", "Title", copy.PublicationId);
            ViewData["StatusId"] = new SelectList(_context.Copystatuses, "Id", "StatusName", copy.StatusId);
            return View(copy);
        }

        // POST: Copies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("PublicationId,Copynumber,StatusId")] Copy copy)
        {
            if (id != copy.PublicationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(copy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CopyExists(copy.PublicationId,copy.Copynumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = copy.PublicationId });
            }
            ViewData["PublicationId"] = new SelectList(_context.Publications, "Id", "Title", copy.PublicationId);
            ViewData["StatusId"] = new SelectList(_context.Copystatuses, "Id", "StatusName", copy.StatusId);
            return View(copy);
        }

        // GET: Copies/Delete/5
        public async Task<IActionResult> Delete(Guid? id, int? copyNum)
        {
            if (id == null || copyNum == null)
            {
                return NotFound();
            }

            var copy = await _context.Copies
                .Include(c => c.Publication)
                .Include(c => c.Status)
                .FirstOrDefaultAsync(m => m.PublicationId == id && m.Copynumber == copyNum);

            if (copy == null)
            {
                return NotFound();
            }

            return View(copy);
        }

        // POST: Copies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid PublicationId, int Copynumber)
        {
            var copy = await _context.Copies.FindAsync(PublicationId, Copynumber);
            if (copy != null)
            {
                _context.Copies.Remove(copy);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { id = PublicationId });
        }
    }
}
