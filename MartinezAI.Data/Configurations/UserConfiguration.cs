using MartinezAI.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MartinezAI.Data.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");

        builder.HasKey(k => k.Id);
        builder.HasIndex(i => i.Email).IsUnique();

        builder.Property(p => p.Email)
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(p => p.Password)
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(p => p.FirstName)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(p => p.LastName)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(p => p.IsAdmin)
            .IsRequired();
    }
}