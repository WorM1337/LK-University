using Microsoft.EntityFrameworkCore;
using Personal_Cabinet_Uni.ExternalInfoService.Models.Entities;
using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.ExternalInfoService.Data;

public class ExternalInfoDbContext : DbContext
{
    public const string SchemaName = "external_info_service";

    public ExternalInfoDbContext(DbContextOptions<ExternalInfoDbContext> options) : base(options)
    {
    }

    public DbSet<EducationLevel> EducationLevels => Set<EducationLevel>();
    public DbSet<Faculty> Faculties => Set<Faculty>();
    public DbSet<EducationDocumentType> EducationDocumentTypes => Set<EducationDocumentType>();
    public DbSet<EducationProgram> EducationPrograms => Set<EducationProgram>();
    public DbSet<DictionaryImportStatus> ImportStatuses => Set<DictionaryImportStatus>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);

        modelBuilder.Entity<EducationLevel>(entity =>
        {
            entity.ToTable("education_levels");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(500).IsRequired();
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.ToTable("faculties");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.CreateTime).HasColumnName("create_time");
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(500).IsRequired();
        });

        modelBuilder.Entity<EducationDocumentType>(entity =>
        {
            entity.ToTable("education_document_types");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.CreateTime).HasColumnName("create_time");
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(500).IsRequired();
            entity.Property(x => x.EducationLevelId).HasColumnName("education_level_id");
            entity.Property(x => x.NextEducationLevelIds).HasColumnName("next_education_level_ids").HasDefaultValue(string.Empty);
            entity.HasOne(x => x.EducationLevel)
                .WithMany()
                .HasForeignKey(x => x.EducationLevelId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EducationProgram>(entity =>
        {
            entity.ToTable("education_programs");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.CreateTime).HasColumnName("create_time");
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(500).IsRequired();
            entity.Property(x => x.Code).HasColumnName("code").HasMaxLength(100);
            entity.Property(x => x.Language).HasColumnName("language").HasMaxLength(100).IsRequired();
            entity.Property(x => x.EducationForm).HasColumnName("education_form").HasMaxLength(100).IsRequired();
            entity.Property(x => x.FacultyId).HasColumnName("faculty_id");
            entity.Property(x => x.EducationLevelId).HasColumnName("education_level_id");
            entity.HasOne(x => x.Faculty)
                .WithMany()
                .HasForeignKey(x => x.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.EducationLevel)
                .WithMany()
                .HasForeignKey(x => x.EducationLevelId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DictionaryImportStatus>(entity =>
        {
            entity.ToTable("import_statuses");
            entity.HasKey(x => x.DictionaryName);
            entity.Property(x => x.DictionaryName).HasColumnName("dictionary_name").HasMaxLength(100);
            entity.Property(x => x.Status)
                .HasColumnName("status")
                .HasConversion(
                    x => x.ToString(),
                    x => Enum.Parse<DictionaryImportingStatus>(x))
                .HasMaxLength(50);
            entity.Property(x => x.ImportedCount).HasColumnName("imported_count");
            entity.Property(x => x.ErrorMessage).HasColumnName("error_message");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        });
    }
}
