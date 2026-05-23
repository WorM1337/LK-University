using Microsoft.EntityFrameworkCore;
using Personal_Cabinet_Uni.DocumentsService.Models.Entities;

namespace Personal_Cabinet_Uni.DocumentsService.Data;

public class DocumentsDbContext : DbContext
{
    public const string Schema = "document_service";

    public DocumentsDbContext(DbContextOptions<DocumentsDbContext> options) : base(options)
    {
    }

    public DbSet<DocumentRecord> Documents => Set<DocumentRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<DocumentRecord>(entity =>
        {
            entity.ToTable("documents");
            entity.HasKey(document => document.Id);

            entity.Property(document => document.Id).HasColumnName("id");
            entity.Property(document => document.OwnerEmail).HasColumnName("owner_email").HasMaxLength(256).IsRequired();
            entity.Property(document => document.DocumentType).HasColumnName("document_type").HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(document => document.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
            entity.Property(document => document.OriginalFileName).HasColumnName("original_file_name").HasMaxLength(512).IsRequired();
            entity.Property(document => document.ContentType).HasColumnName("content_type").HasMaxLength(128).IsRequired();
            entity.Property(document => document.RelativePath).HasColumnName("relative_path").HasMaxLength(1024).IsRequired();
            entity.Property(document => document.Size).HasColumnName("size").IsRequired();
            entity.Property(document => document.PassportSeries).HasColumnName("passport_series").HasMaxLength(32);
            entity.Property(document => document.PassportNumber).HasColumnName("passport_number").HasMaxLength(64);
            entity.Property(document => document.BirthPlace).HasColumnName("birth_place").HasMaxLength(256);
            entity.Property(document => document.IssuedAt).HasColumnName("issued_at");
            entity.Property(document => document.IssuedBy).HasColumnName("issued_by").HasMaxLength(512);
            entity.Property(document => document.EducationDocumentName).HasColumnName("education_document_name").HasMaxLength(256);
            entity.Property(document => document.EducationLevelName).HasColumnName("education_level_name").HasMaxLength(256);
            entity.Property(document => document.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(document => document.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(document => document.OwnerEmail).HasDatabaseName("ix_documents_owner_email");
            entity.HasIndex(document => new { document.OwnerEmail, document.DocumentType }).HasDatabaseName("ix_documents_owner_email_document_type");
        });
    }
}
