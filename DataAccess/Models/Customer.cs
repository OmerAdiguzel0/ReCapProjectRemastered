using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Rentals = new HashSet<Rental>();
        }

        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public string CompanyName { get; set; } = null!;

        public virtual User User { get; set; } = null!;
        public virtual ICollection<Rental> Rentals { get; set; }
    }
}
