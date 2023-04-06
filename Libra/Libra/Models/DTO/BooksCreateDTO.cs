using System.ComponentModel.DataAnnotations;

namespace Tizi_sBookStore.Models.DTO
{
    public class BooksCreateDTO

    {
       
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        [Required]
        public IFormFile File { get; set; }
        public int AuthorId { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
}
