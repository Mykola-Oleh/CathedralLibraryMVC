using CathedralLibraryDomain.Model;
using DocumentFormat.OpenXml.Vml.Office;

namespace CathedralLibraryInfrastructure.Services
{
    public interface IDataPortServiceFactory<TEntity> where TEntity : class
    {
        IImportService<TEntity> GetImportService(string contentType);
        IExportService<TEntity> GetExportService(string contentType);
    }
}
