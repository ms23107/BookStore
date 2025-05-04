using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Review
    {
        public int Id { get; set; }

        public string UserEmail { get; set; } 

        [Required]
        public string Comment { get; set; } 

        [Required]
        [Range(1, 5, ErrorMessage = "Vērtējumam ir jābūt no 1 līdz 5.")]
        public int Rating { get; set; }

        public DateTime ReviewDate { get; set; } = DateTime.Now; 

        public int BookId { get; set; }
        public Book Book { get; set; } 

    }
}
