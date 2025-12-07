using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class LoanDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int MemberId { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }

    public class CreateLoanDto
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Required]
        public DateTime DueDate { get; set; }
    }

    public class UpdateLoanDto
    {
        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }
    }
}
