using MartinezAI.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MartinezAI.Data.Configurations;

internal class StringScalarResultConfiguration : IEntityTypeConfiguration<StringScalarResult>
{
    public void Configure(EntityTypeBuilder<StringScalarResult> builder)
    {
        builder.ToView(null);
        builder.HasNoKey();
    }
}
