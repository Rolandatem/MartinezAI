using MartinezAI.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MartinezAI.Data.Configurations;

internal class UserAssistantThreadConfiguration : IEntityTypeConfiguration<UserAssistantThread>
{
    public void Configure(EntityTypeBuilder<UserAssistantThread> builder)
    {
        builder.ToTable("UserAssistantThread");

        builder.HasKey(k => k.Id);
        builder.HasIndex(i => new { i.ThreadName, i.AssistantId}).IsUnique();

        builder.Property(p => p.ThreadName)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(p => p.AssistantId)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(p => p.ThreadId)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(p => p.LastMessageId).HasMaxLength(100);
    }
}
