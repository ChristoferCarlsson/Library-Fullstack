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
        public int BookId { get; set; }
        public int MemberId { get; set; }

        public DateTime DueDate { get; set; }
    }

    public class UpdateLoanDto
    {
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
