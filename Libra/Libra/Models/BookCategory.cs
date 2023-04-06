using System.ComponentModel.DataAnnotations;

namespace Libra.Models
{
    public class BookCategory
    {
        
        public int BookId { get; set; }
        public Books Book { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
