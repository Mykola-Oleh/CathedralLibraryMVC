using CathedralLibraryDomain.Model;

namespace CathedralLibraryInfrastructure.Services
{
    public class PublicationDataPortServiceFactory : IDataPortServiceFactory<Publication>
    {
        private readonly DbCathedralLibraryContext _context;
        public PublicationDataPortServiceFactory(DbCathedralLibraryContext context) => _context = context;

        public IImportService<Publication> GetImportService(string contentType)
        {
            if (contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                return new PublicationImportService(_context);
            throw new NotImplementedException("Формат не підтримується");
        }

        public IExportService<Publication> GetExportService(string contentType)
        {
            if (contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                return new PublicationExportService(_context);
            throw new NotImplementedException("Формат не підтримується");
        }
    }
}