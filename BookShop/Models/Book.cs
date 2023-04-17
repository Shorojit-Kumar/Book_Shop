using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShop.Models
{
    public class Book
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }

        public DateTime ? PublishedAt { get; set; } = default(DateTime?);

        [ForeignKey(nameof(AuthorId))]
        [Required]
        public int AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public Author? Author { get; set; }
     
        public string? BookImageUrl { get; set; } = "/images/DefaultBook.jpg";
        public DateTime? CreatedAt { get; set; } = default(DateTime?);
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;


    }
}
