using System.ComponentModel.DataAnnotations;

namespace Libra.Models.DTO
{
    public class BooksDTO
    {

    
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int? AuthorId { get; set; }
        [Required]
        public IFormFile File { get; set; }
        public int[] CategoryId { get; set; }
        public string CreatedBy { get; set; }

        //public List<CategoryCreateDTO> Category { get; set; }


    }
}
