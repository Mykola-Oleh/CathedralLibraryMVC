using CathedralLibraryDomain.Model;
using DocumentFormat.OpenXml.Vml.Office;

namespace CathedralLibraryInfrastructure.Services
{
    public interface IExportService<TEntity> where TEntity : class
    {
        Task WriteToAsync(Stream stream, CancellationToken cancellationToken);
    }
}
