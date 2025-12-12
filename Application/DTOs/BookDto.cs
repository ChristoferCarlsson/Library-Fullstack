using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public DateTime PublishedDate { get; set; }
        public int AuthorId { get; set; }
        public required string AuthorFullName { get; set; }
        public required string ISBN { get; set; }
        public int CopiesAvailable { get; set; }
        public int CopiesTotal { get; set; }
    }

    public class CreateBookDto
    {
        [Required]
        [MaxLength(200)]
        public required string Title { get; set; }

        [Required]
        public DateTime PublishedDate { get; set; }

        [Required]
        public int AuthorId { get; set; }

        [MaxLength(50)]
        public required string ISBN { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "CopiesTotal must be at least 1.")]
        public int CopiesTotal { get; set; }
    }

    public class UpdateBookDto
    {
        [Required]
        [MaxLength(200)]
        public required string Title { get; set; }

        [Required]
        public DateTime PublishedDate { get; set; }

        [Required]
        public int AuthorId { get; set; }

        [MaxLength(50)]
        public required string ISBN { get; set; }

        [Range(1, int.MaxValue)]
        public int CopiesTotal { get; set; }
    }

}
