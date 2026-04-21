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

        public async Task ImportFromStreamAsync(Stream stream, Guid adminId, CancellationToken cancellationToken)
        {
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
                        await _context.SaveChangesAsync(cancellationToken);
                    }

                    foreach (var row in worksheet.RowsUsed().Skip(1))
                    {
                        var titleCell = row.Cell(1).Value;
                        if (titleCell.IsBlank || string.IsNullOrWhiteSpace(titleCell.ToString()))
                        {
                            continue;
                        }
                        string title = titleCell.ToString().Trim();

                        var yearCell = row.Cell(2).Value;
                        short year = 0;

                        if (yearCell.IsNumber)
                        {
                            year = (short)yearCell.GetNumber();
                        }
                        else if (yearCell.IsDateTime)
                        {
                            year = (short)yearCell.GetDateTime().Year;
                        }
                        else
                        {
                            string yearStr = yearCell.ToString();
                            if (DateTime.TryParse(yearStr, out DateTime parsedDate))
                            {
                                year = (short)parsedDate.Year;
                            }
                            else if (!short.TryParse(yearStr, out year))
                            {
                                continue;
                            }
                        }

                        short currentYear = (short)DateTime.Now.Year;
                        if (year < 1000 || year > currentYear + 1)
                        {
                            continue;
                        }
                        var publication = await _context.Publications
                            .FirstOrDefaultAsync(p => p.Title == title && p.Year == year, cancellationToken);

                        if (publication != null)
                        {
                            continue;
                        }

                        string annotation = row.Cell(3).Value.IsBlank ? "" : row.Cell(3).Value.ToString();

                        var newPublication = new Publication
                        {
                            Title = title,
                            Year = year,
                            Annotation = annotation,
                            PublicationTypeId = pubType.Id
                        };

                        _context.Publications.Add(newPublication);

                        await _context.SaveChangesAsync(cancellationToken);

                        var fundChange = new Fundchange
                        {
                            Id = Guid.NewGuid(),
                            PublicationId = newPublication.Id,
                            PublicationTitle = title,
                            ChangeType = "Імпорт видання",
                            Quantity = 0,
                            ChangeDate = DateOnly.FromDateTime(DateTime.Now),
                            ChangedByAdminId = adminId
                        };

                        _context.Fundchanges.Add(fundChange);
                    }
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}