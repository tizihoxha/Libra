using System.ComponentModel.DataAnnotations;

namespace Libra.Models.DTO
{
    public class AuthorCreateDTO
    {
        [Required]
        public string Name { get; set; }

        public string Bio { get; set; }
        public string CreatedBy { get; set; }
        public int BookCount { get; set; }


    }
}
