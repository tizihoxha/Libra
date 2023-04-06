using System.ComponentModel.DataAnnotations;

namespace Libra.Models.DTO
{
    public class CategoryCreateDTO
    {
        [Required]
        public string Name { get; set; }
        public string Priority { get; set; }
        public string CreatedBy { get; set; }
    }
}
