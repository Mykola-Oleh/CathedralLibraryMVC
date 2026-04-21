namespace CathedralLibraryInfrastructure.Services
{
    public interface IImportService<TEntity> where TEntity : class
    {
        Task ImportFromStreamAsync(Stream stream, Guid adminId, CancellationToken cancellationToken);
    }
}
