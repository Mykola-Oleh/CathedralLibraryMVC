using CathedralLibraryDomain.Model;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibraryInfrastructure;

public partial class DbCathedralLibraryContext : DbContext
{
    public DbCathedralLibraryContext(DbContextOptions<DbCathedralLibraryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }
    public virtual DbSet<Copy> Copies { get; set; }
    public virtual DbSet<Copystatus> Copystatuses { get; set; }
    public virtual DbSet<Fundchange> Fundchanges { get; set; }
    public virtual DbSet<Publication> Publications { get; set; }
    public virtual DbSet<Reader> Readers { get; set; }
    public virtual DbSet<Request> Requests { get; set; }
    public virtual DbSet<Requeststatus> Requeststatuses { get; set; }
    public virtual DbSet<Wishlist> Wishlists { get; set; }

    public virtual DbSet<PublicationType> PublicationType { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var idProperty = entity.FindProperty("Id");
            if (idProperty != null && idProperty.ClrType == typeof(Guid))
            {
                idProperty.SetDefaultValueSql("uuid_generate_v7()");
            }
        }

        modelBuilder.Entity<Publication>(entity =>
        {
            entity.HasOne(d => d.PublicationType)
                  .WithMany(p => p.Publications)
                  .HasForeignKey(d => d.PublicationTypeId)
                  .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Copy>(entity =>
        {
            entity.HasKey(e => new { e.PublicationId, e.Copynumber });

            entity.HasOne(d => d.Status).WithMany(p => p.Copies)
                .HasForeignKey(d => d.StatusId);
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasOne(d => d.Copy).WithMany(p => p.Requests)
                .HasForeignKey(d => new { d.PublicationId, d.Copynumber });
        });

        base.OnModelCreating(modelBuilder);
    }
}