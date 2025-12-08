using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class MemberDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public required string Email { get; set; }
        public DateTime JoinedAt { get; set; }
        public int LoanCount { get; set; }


    }

    public class CreateMemberDto
    {
        [Required]
        [MaxLength(100)]
        public required string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public required string LastName { get; set; }

        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public required string Email { get; set; }
    }

    public class UpdateMemberDto
    {
        [Required]
        [MaxLength(100)]
        public required string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public required string LastName { get; set; }

        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public required string Email { get; set; }
    }
}
