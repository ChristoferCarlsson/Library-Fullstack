using System;

namespace Application.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime PublishedDate { get; set; }

        public int AuthorId { get; set; }
        public string AuthorFullName { get; set; }

        public string ISBN { get; set; }
        public int CopiesAvailable { get; set; }
        public int CopiesTotal { get; set; }
    }

    public class CreateBookDto
    {
        public string Title { get; set; }
        public DateTime PublishedDate { get; set; }

        public int AuthorId { get; set; }

        public string ISBN { get; set; }
        public int CopiesTotal { get; set; }
    }

    public class UpdateBookDto
    {
        public string Title { get; set; }
        public DateTime PublishedDate { get; set; }

        public int AuthorId { get; set; }

        public string ISBN { get; set; }
        public int CopiesAvailable { get; set; }
        public int CopiesTotal { get; set; }
    }
}
