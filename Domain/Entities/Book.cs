using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Book : BaseEntity
    {
        public string Title { get; set; }
        public DateTime PublishedDate { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }

        public string ISBN { get; set; }
        public int CopiesAvailable { get; set; }
        public int CopiesTotal { get; set; }

        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
