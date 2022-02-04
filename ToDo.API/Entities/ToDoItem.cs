using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDo.API.Entities
{
    public class ToDoItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(30)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Range(1, 3)]
        public int Priority { get; set; }

        public bool IsCompleted { get; set; }
    }
}
