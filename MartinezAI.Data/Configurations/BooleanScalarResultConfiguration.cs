using MartinezAI.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MartinezAI.Data.Configurations;

internal class BooleanScalarResultConfiguration : IEntityTypeConfiguration<BooleanScalarResult>
{
    public void Configure(EntityTypeBuilder<BooleanScalarResult> builder)
    {
        builder.ToView(null);
        builder.HasNoKey();

        builder.Property(p => p.Value).IsRequired();
    }
}
