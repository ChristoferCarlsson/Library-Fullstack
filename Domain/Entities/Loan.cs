using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Loan : BaseEntity
    {
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;
        public DateTime LoanDate { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
