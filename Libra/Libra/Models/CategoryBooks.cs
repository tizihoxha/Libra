namespace Libra.Models
{
    public class CategoryBooks
    {
        public int BookId { get; set; }
        public Books Book { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
