using MartinezAI.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MartinezAI.Data.Configurations;

internal class UserViewConfiguration : IEntityTypeConfiguration<UserView>
{
    public void Configure(EntityTypeBuilder<UserView> builder)
    {
        builder.ToView("UserView");
        builder.HasNoKey();

        builder.Property(p => p.Email)
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(p => p.FirstName)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(p => p.LastName)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(p => p.IsAdmin).IsRequired();
    }
}
