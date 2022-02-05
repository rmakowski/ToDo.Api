using Microsoft.EntityFrameworkCore;
using ToDo.API.Entities;

namespace ToDo.API.Contexts;

public class ToDoContext : DbContext
{
    public ToDoContext(DbContextOptions<ToDoContext> options) : base(options)
    {
    }

    public DbSet<ToDoItem> ToDoItems { get; set; } = null!;
}
