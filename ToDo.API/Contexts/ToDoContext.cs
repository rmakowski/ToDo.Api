using Microsoft.EntityFrameworkCore;
using ToDo.API.Entities;

namespace ToDo.API.Contexts;

public class ToDoContext : DbContext
{
    public ToDoContext(DbContextOptions<ToDoContext> options) : base(options)
    {
    }

    public DbSet<ToDoItem> ToDoItems { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ToDoItem>()
            .HasOne(toDoItem => toDoItem.User)
            .WithMany(user => user.ToDoItems)
            .HasForeignKey(toDoItem => toDoItem.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
