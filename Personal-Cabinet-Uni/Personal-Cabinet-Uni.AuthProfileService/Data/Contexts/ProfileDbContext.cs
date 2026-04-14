using Microsoft.EntityFrameworkCore;
using Personal_Cabinet_Uni.Models.Entities;

namespace Personal_Cabinet_Uni.Data.Contexts;

public class ProfileDbContext : DbContext
{
    public ProfileDbContext(DbContextOptions<ProfileDbContext> options) : base(options)
    {
    }
    
    public DbSet<Profile> Profiles { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProfileDbContext).Assembly);
    }
}
