using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoListApp.Domain.TodoItems;

namespace TodoListApp.Infrastructure.Persistence.Configurations;

public sealed class TodoItemTagConfiguration : IEntityTypeConfiguration<TodoItemTag>
{
    public void Configure(EntityTypeBuilder<TodoItemTag> builder)
    {
        builder.ToTable("todo_item_tags");
        
        builder.HasKey(tt => new { tt.TodoItemId, tt.TagId });
        
        builder.HasOne(tt => tt.TodoItem)
            .WithMany(t => t.TodoItemTags)
            .HasForeignKey(tt => tt.TodoItemId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(tt => tt.Tag)
            .WithMany(t => t.TodoItemTags)
            .HasForeignKey(tt => tt.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
