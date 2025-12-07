using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Member : BaseEntity
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
