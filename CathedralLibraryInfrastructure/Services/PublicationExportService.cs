using CathedralLibraryDomain.Model;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibraryInfrastructure.Services
{
    public class PublicationExportService : IExportService<Publication>
    {
        private static readonly string[] HeaderNames = { "Назва видання", "Рік", "Анотація" };
        private readonly DbCathedralLibraryContext _context;

        public PublicationExportService(DbCathedralLibraryContext context)
        {
            _context = context;
        }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            var types = await _context.PublicationType
                .Include(t => t.Publications)
                .ToListAsync(cancellationToken);

            using (var workbook = new XLWorkbook())
            {
                foreach (var type in types)
                {
                    var worksheet = workbook.Worksheets.Add(type.Name);

                    for (int i = 0; i < HeaderNames.Length; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = HeaderNames[i];
                        worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                    }

                    int rowIndex = 2;
                    foreach (var pub in type.Publications)
                    {
                        worksheet.Cell(rowIndex, 1).Value = pub.Title;
                        worksheet.Cell(rowIndex, 2).Value = pub.Year;
                        worksheet.Cell(rowIndex, 3).Value = pub.Annotation;
                        rowIndex++;
                    }
                    worksheet.Columns().AdjustToContents();
                }
                workbook.SaveAs(stream);
            }
        }
    }
}