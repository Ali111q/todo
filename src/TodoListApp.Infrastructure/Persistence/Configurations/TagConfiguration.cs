using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoListApp.Domain.TodoItems;

namespace TodoListApp.Infrastructure.Persistence.Configurations;

public sealed class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tags");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(t => t.Color)
            .IsRequired()
            .HasMaxLength(7)
            .HasDefaultValue("#007bff");
            
        builder.Property(t => t.UserId)
            .IsRequired();
            
        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasConversion(
                v => v.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v, DateTimeKind.Utc) : v,
                v => v
            );

        builder.Property(t => t.UpdatedAt)
            .IsRequired()
            .HasConversion(
                v => v.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v, DateTimeKind.Utc) : v,
                v => v
            );

        builder.HasIndex(t => new { t.UserId, t.Name })
            .IsUnique();
    }
}
