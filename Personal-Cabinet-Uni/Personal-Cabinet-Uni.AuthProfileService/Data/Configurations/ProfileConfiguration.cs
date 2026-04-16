using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Personal_Cabinet_Uni.Models.Entities;
using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.Data.Configurations;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("profiles");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        
        builder.Property(p => p.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(p => p.Surname)
            .HasColumnName("surname")
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(p => p.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(100);
        
        builder.Property(p => p.Email)
            .HasColumnName("email")
            .IsRequired()
            .HasMaxLength(256);
        
        builder.HasIndex(p => p.Email)
            .IsUnique();
        
        builder.Property(p => p.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20);
        
        builder.Property(p => p.Birthday)
            .HasColumnName("birthday");
        
        builder.Property(p => p.Gender)
            .HasColumnName("gender");
        
        builder.Property(p => p.Nationality)
            .HasColumnName("nationality")
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(p => p.Password)
            .HasColumnName("password")
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(p => p.Role)
            .HasColumnName("role")
            .IsRequired();
        
        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at");
        
        builder.Property(p => p.RefreshToken)
            .HasColumnName("refresh_token")
            .HasMaxLength(256);

        builder.HasIndex(p => p.RefreshToken)
            .IsUnique();
        
        builder.HasIndex(p => p.Role);
    }
}
