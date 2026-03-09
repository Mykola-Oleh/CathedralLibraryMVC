using System;
using System.Collections.Generic;
using CathedralLibraryDomain.Model;
using Microsoft.EntityFrameworkCore;

namespace CathedralLibraryInfrastructure;

public partial class DbCathedralLibraryContext : DbContext
{
    public DbCathedralLibraryContext()
    {
    }

    public DbCathedralLibraryContext(DbContextOptions<DbCathedralLibraryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Copy> Copys { get; set; }

    public virtual DbSet<Copystatus> Copystatuses { get; set; }

    public virtual DbSet<Fundchange> Fundchanges { get; set; }

    public virtual DbSet<Publication> Publications { get; set; }

    public virtual DbSet<Reader> Readers { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<Requeststatus> Requeststatuses { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=CathedralLibDB;Username=postgres;Password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("authors_pkey");

            entity.ToTable("authors");

            entity.Property(e => e.AuthorId)
                .ValueGeneratedNever()
                .HasColumnName("author_id");
            entity.Property(e => e.FirstName)
                .HasMaxLength(64)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(64)
                .HasColumnName("last_name");

            entity.HasMany(d => d.Publications).WithMany(p => p.Authors)
                .UsingEntity<Dictionary<string, object>>(
                    "Publiactionauthor",
                    r => r.HasOne<Publication>().WithMany()
                        .HasForeignKey("PublicationId")
                        .HasConstraintName("publiactionauthors_publication_id_fkey"),
                    l => l.HasOne<Author>().WithMany()
                        .HasForeignKey("AuthorId")
                        .HasConstraintName("publiactionauthors_author_id_fkey"),
                    j =>
                    {
                        j.HasKey("AuthorId", "PublicationId").HasName("publiactionauthors_pkey");
                        j.ToTable("publiactionauthors");
                        j.IndexerProperty<int>("AuthorId").HasColumnName("author_id");
                        j.IndexerProperty<int>("PublicationId").HasColumnName("publication_id");
                    });
        });

        modelBuilder.Entity<Copy>(entity =>
        {
            entity.HasKey(e => new { e.PublicationId, e.Copynumber }).HasName("copys_pkey");

            entity.ToTable("copys");

            entity.Property(e => e.PublicationId).HasColumnName("publication_id");
            entity.Property(e => e.Copynumber).HasColumnName("copynumber");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.Publication).WithMany(p => p.Copies)
                .HasForeignKey(d => d.PublicationId)
                .HasConstraintName("copys_publication_id_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.Copies)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("copys_status_id_fkey");
        });

        modelBuilder.Entity<Copystatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("copystatus_pkey");

            entity.ToTable("copystatus");

            entity.Property(e => e.StatusId)
                .ValueGeneratedNever()
                .HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(32)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<Fundchange>(entity =>
        {
            entity.HasKey(e => e.ChangeId).HasName("fundchanges_pkey");

            entity.ToTable("fundchanges");

            entity.Property(e => e.ChangeId)
                .ValueGeneratedNever()
                .HasColumnName("change_id");
            entity.Property(e => e.ChangeDate).HasColumnName("change_date");
            entity.Property(e => e.ChangeType)
                .HasMaxLength(64)
                .HasColumnName("change_type");
            entity.Property(e => e.ChangedByAdminId).HasColumnName("changed_by_admin_id");
            entity.Property(e => e.PublicationId).HasColumnName("publication_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Publication).WithMany(p => p.Fundchanges)
                .HasForeignKey(d => d.PublicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fundchanges_publication_id_fkey");
        });

        modelBuilder.Entity<Publication>(entity =>
        {
            entity.HasKey(e => e.PublicationId).HasName("publications_pkey");

            entity.ToTable("publications");

            entity.Property(e => e.PublicationId)
                .ValueGeneratedNever()
                .HasColumnName("publication_id");
            entity.Property(e => e.Anotaion)
                .HasMaxLength(255)
                .HasColumnName("anotaion");
            entity.Property(e => e.PublicationType)
                .HasMaxLength(32)
                .HasColumnName("publication_type");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Year).HasColumnName("year");
        });

        modelBuilder.Entity<Reader>(entity =>
        {
            entity.HasKey(e => e.ReaderId).HasName("readers_pkey");

            entity.ToTable("readers");

            entity.Property(e => e.ReaderId)
                .ValueGeneratedNever()
                .HasColumnName("reader_id");
            entity.Property(e => e.Department)
                .HasMaxLength(128)
                .HasColumnName("department");
            entity.Property(e => e.Faculty)
                .HasMaxLength(128)
                .HasColumnName("faculty");
            entity.Property(e => e.FirstName)
                .HasMaxLength(64)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(64)
                .HasColumnName("last_name");
            entity.Property(e => e.Position)
                .HasMaxLength(255)
                .HasColumnName("position");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("requests_pkey");

            entity.ToTable("requests");

            entity.Property(e => e.RequestId)
                .ValueGeneratedNever()
                .HasColumnName("request_id");
            entity.Property(e => e.Copynumber).HasColumnName("copynumber");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.PublicationId).HasColumnName("publication_id");
            entity.Property(e => e.ReaderId).HasColumnName("reader_id");
            entity.Property(e => e.RequestDate).HasColumnName("request_date");
            entity.Property(e => e.ReturnDate).HasColumnName("return_date");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.Reader).WithMany(p => p.Requests)
                .HasForeignKey(d => d.ReaderId)
                .HasConstraintName("requests_reader_id_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.Requests)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("requests_status_id_fkey");

            entity.HasOne(d => d.Copy).WithMany(p => p.Requests)
                .HasForeignKey(d => new { d.PublicationId, d.Copynumber })
                .HasConstraintName("requests_publication_id_copynumber_fkey");
        });

        modelBuilder.Entity<Requeststatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("requeststatus_pkey");

            entity.ToTable("requeststatus");

            entity.Property(e => e.StatusId)
                .ValueGeneratedNever()
                .HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(32)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.ReaderId).HasName("wishlist_pkey");

            entity.ToTable("wishlist");

            entity.Property(e => e.ReaderId)
                .ValueGeneratedNever()
                .HasColumnName("reader_id");
            entity.Property(e => e.PublicationId).HasColumnName("publication_id");

            entity.HasOne(d => d.Publication).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.PublicationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("wishlist_publication_id_fkey");

            entity.HasOne(d => d.Reader).WithOne(p => p.Wishlist)
                .HasForeignKey<Wishlist>(d => d.ReaderId)
                .HasConstraintName("wishlist_reader_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
