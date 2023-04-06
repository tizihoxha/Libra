using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Libra.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Priority { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedBy { get; set;}
        [JsonIgnore]
        public  ICollection<CategoryBooks> CategoryBooks { get; set; }
    }
}
