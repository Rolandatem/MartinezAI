using MartinezAI.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MartinezAI.Data;

public class MartinezAIDbContext : DbContext
{
    #region "Member Variables"
    #endregion

    #region "Tables"
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserAssistantThread> UserAssistantThreads { get; set; } = null!;
    #endregion

    #region "Views"
    public DbSet<UserView> View_Users { get; set; } = null!;
    #endregion

    #region "Scalar"
    public DbSet<BooleanScalarResult> BooleanScalarResults { get; set; } = null!;
    public DbSet<StringScalarResult> StringScalarResults { get; set; } = null!;
    #endregion

    #region "Overrides"
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder
            .UseNpgsql("Host=localhost;Port=5432;Database=martinezaidb;Username=martinezai;Password=MartinezAI#1");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        Assembly assembly = GetType().Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
    }
    #endregion
}
