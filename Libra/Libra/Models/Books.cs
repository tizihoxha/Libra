using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Libra.Models
{
    public class Books
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedBy{ get; set; }
        [Required]
        public string Image { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public ICollection<CategoryBooks> CategoryBooks { get; set; }
    }
}
