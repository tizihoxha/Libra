namespace Libra.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedBy { get; set; }

        public ICollection<Books> Book { get; set; } = new List<Books>();
    }
}
