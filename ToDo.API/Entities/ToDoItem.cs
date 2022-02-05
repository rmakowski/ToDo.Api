using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDo.API.Entities
{
    public class ToDoItem
    {
        /// <example>123</example>>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <example>To do item name</example>>
        [MaxLength(30)]
        public string Name { get; set; } = null!;

        /// <example>To do item description</example>>
        public string? Description { get; set; }

        /// <example>3</example>>
        [Range(1, 3)]
        public int Priority { get; set; }

        /// <example>false</example>>
        public bool IsCompleted { get; set; }
    }
}
