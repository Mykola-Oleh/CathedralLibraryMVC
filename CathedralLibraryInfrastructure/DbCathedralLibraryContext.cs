using CathedralLibraryDomain.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibraryInfrastructure;

public partial class DbCathedralLibraryContext : IdentityDbContext<User>
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
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PublicationType>(entity =>
        {
            entity.ToTable("publication_type");
        });

        modelBuilder.Entity<Publication>(entity =>
        {
            entity.ToTable("publications");

            entity.HasOne(d => d.PublicationType)
                  .WithMany(p => p.Publications)
                  .HasForeignKey(d => d.PublicationTypeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Copy>(entity =>
        {
            entity.ToTable("copies");

            entity.HasKey(e => new { e.PublicationId, e.Copynumber });

            entity.HasOne(d => d.Publication)
                .WithMany(p => p.Copies)
                .HasForeignKey(d => d.PublicationId);

            entity.HasOne(d => d.Status)
                .WithMany(p => p.Copies)
                .HasForeignKey(d => d.StatusId);
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.ToTable("requests");

            entity.HasOne(d => d.Copy)
                .WithMany(p => p.Requests)
                .HasForeignKey(d => new { d.PublicationId, d.Copynumber });

            entity.HasOne(d => d.Reader)
                .WithMany(p => p.Requests)
                .HasForeignKey(d => d.ReaderId);

            entity.HasOne(d => d.Status)
                .WithMany(p => p.Requests)
                .HasForeignKey(d => d.StatusId);
        });

        modelBuilder.Entity<Reader>(entity =>
        {
            entity.ToTable("readers");

            entity.HasOne(r => r.User)
                .WithOne(u => u.Reader)
                .HasForeignKey<Reader>(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.ToTable("authors");

            entity.HasMany(a => a.Publications)
                .WithMany(p => p.Authors)
                .UsingEntity<Dictionary<string, object>>(
                    "publicationauthors",
                    j => j.HasOne<Publication>().WithMany().HasForeignKey("publication_id"),
                    j => j.HasOne<Author>().WithMany().HasForeignKey("author_id")
                );
        });
        modelBuilder.Entity<Fundchange>(entity =>
        {
            entity.ToTable("fundchanges");
            entity.HasOne(d => d.Publication)
                  .WithMany(p => p.Fundchanges)
                  .HasForeignKey(d => d.PublicationId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var idProperty = entity.FindProperty("Id");
            if (idProperty != null && idProperty.ClrType == typeof(Guid))
            {
                idProperty.SetDefaultValueSql("uuid_generate_v7()");
            }
        }
    }
}