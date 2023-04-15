using System.ComponentModel.DataAnnotations;

namespace BookShop.Models
{
    public class Author
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }
        public string? Biography { get; set; }
        public string? Address { get; set; }
        public string AuthorImageURL { get; set; } = "/images/Default.jpg";
        public List<Book>? BookList { get; set;}=new List<Book>();
    }
}
