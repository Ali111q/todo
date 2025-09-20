using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoListApp.Domain.TodoItems;

namespace TodoListApp.Infrastructure.Persistence.Configurations;

public sealed class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> b)
    {
        b.ToTable("todo_items");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).IsRequired();
        b.Property(x => x.UserId).IsRequired();
        b.Property(x => x.IsCompleted).IsRequired();

        b.Property(x => x.CreatedAt)
            .IsRequired()
            .HasConversion(
                v => v.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v, DateTimeKind.Utc) : v,
                v => v
            );

        b.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasConversion(
                v => v.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v, DateTimeKind.Utc) : v,
                v => v
            );

        b.Property(x => x.CompletedAtUtc)
            .HasConversion(
                v => v.HasValue && v.Value.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                    : v,
                v => v
            );

        b.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        b.Property(x => x.DeletedAtUtc)
            .HasConversion(
                v => v.HasValue && v.Value.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                    : v,
                v => v
            );

        b.OwnsOne(x => x.Name, nb =>
        {
            nb.Property(p => p.Value).HasColumnName("name").HasMaxLength(120).IsRequired();
        });
        b.OwnsOne(x => x.Description, nb =>
        {
            nb.Property(p => p.Value).HasColumnName("description").HasMaxLength(1000);
        });
        b.OwnsOne(x => x.DueDate, nb =>
        {
            nb.Property(p => p.Value)
                .HasColumnName("due_date")
                .HasConversion(
                    v => v.HasValue && v.Value.Kind == DateTimeKind.Unspecified
                        ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                        : v,
                    v => v
                );
        });
        b.OwnsOne(x => x.Priority, pb =>
        {
            pb.Property(p => p.Value)
                .HasColumnName("priority")
                .HasConversion<string>()
                .IsRequired()
                .HasDefaultValue(Domain.TodoItems.ValueObjects.PriorityLevel.Medium);
        });

        b.HasIndex(x => new { x.UserId, x.IsCompleted });
    }
}
