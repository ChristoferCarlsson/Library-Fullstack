using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class AuthorDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string? Description { get; set; }
    }

    public class CreateAuthorDto
    {
        [Required]
        [MaxLength(100)]
        public required string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public required string LastName { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }

    public class UpdateAuthorDto
    {
        [Required]
        [MaxLength(100)]
        public required string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public required string LastName { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
