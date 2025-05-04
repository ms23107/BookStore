using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Book
    {   
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public string Manufacturer { get; set; }
        public string Language { get; set; }
        [Required(ErrorMessage = "ISBN ir obligāts lauks")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "ISBN var saturēt tikai ciparus")]
        [StringLength(13, MinimumLength = 10, ErrorMessage = "ISBN jābūt no 10 līdz 13 cipariem")]
        public string ISBN { get; set; }
        [Required, DataType(DataType.Date), Display(Name = "Date Published")]
        public DateTime DatePublished { get; set; }
        [Required(ErrorMessage = "Cena ir obligāts lauks")]
        [DataType(DataType.Currency)]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cenai jābūt pozitīvai vērtībai")]
        public int Price { get; set; }
        [Required]
        public string Author { get; set; }
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
