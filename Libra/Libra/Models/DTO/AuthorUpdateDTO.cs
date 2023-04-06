using System.ComponentModel.DataAnnotations;
using System.Security;

namespace Libra.Models.DTO
{
    public class AuthorUpdateDTO
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Bio { get; set; }
        public string CreatedBy { get; set; }
        public int BookCount { get; set; }

    }
}
