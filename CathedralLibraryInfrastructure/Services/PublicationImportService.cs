using CathedralLibraryDomain.Model;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibraryInfrastructure.Services
{
    public class PublicationImportService : IImportService<Publication>
    {
        private readonly DbCathedralLibraryContext _context;

        public PublicationImportService(DbCathedralLibraryContext context)
        {
            _context = context;
        }

        public async Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanRead) throw new ArgumentException("Дані не можуть бути прочитані", nameof(stream));

            using (var workBook = new XLWorkbook(stream))
            {
                foreach (var worksheet in workBook.Worksheets)
                {
                    var typeName = worksheet.Name;
                    var pubType = await _context.PublicationType
                        .FirstOrDefaultAsync(t => t.Name == typeName, cancellationToken);

                    if (pubType == null)
                    {
                        pubType = new PublicationType { Name = typeName };
                        _context.PublicationType.Add(pubType);
                    }

                    foreach (var row in worksheet.RowsUsed().Skip(1))
                    {
                        var titleCell = row.Cell(1).Value;
                        var yearCell = row.Cell(2).Value;
                        if (titleCell.IsBlank) continue;
                        if (!yearCell.IsNumber)
                        {
                            continue;
                        }

                        short year = (short)yearCell.GetNumber();
                        string title = titleCell.ToString();
                        if (title.Length > 255) title = title.Substring(0, 255);
                        if (string.IsNullOrEmpty(title)) continue;

                        var publication = await _context.Publications
                            .FirstOrDefaultAsync(p => p.Title == title, cancellationToken);

                        if (publication == null)
                        {
                            publication = new Publication
                            {
                                Title = title,
                                Year = (short)row.Cell(2).Value.GetNumber(),
                                Annotation = row.Cell(3).Value.ToString(),
                                PublicationType = pubType
                            };
                            _context.Publications.Add(publication);
                        }
                    }
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
