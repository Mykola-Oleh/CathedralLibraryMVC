using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibraryInfrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ChartsController : ControllerBase
    {
        private readonly DbCathedralLibraryContext _context;

        public ChartsController(DbCathedralLibraryContext context)
        {
            _context = context;
        }

        private record CopiesByEditionItem(string EditionTitle, int Count);
        private record EditionsByTypeItem(string TypeName, int Count);

        [HttpGet("copiesByEdition")]
        public async Task<JsonResult> GetCopiesByEdition(CancellationToken ct)
        {
            var data = await _context.Publications
                .Select(b => new CopiesByEditionItem(b.Title, b.Copies.Count))
                .Take(10)
                .ToListAsync(ct);
            return new JsonResult(data);
        }

        [HttpGet("editionsByType")]
        public async Task<JsonResult> GetEditionsByType(CancellationToken ct)
        {
            var data = await _context.Publications
                .GroupBy(b => b.PublicationType.Name)
                .Select(g => new EditionsByTypeItem(g.Key, g.Count()))
                .ToListAsync(ct);
            return new JsonResult(data);
        }
    }
}
