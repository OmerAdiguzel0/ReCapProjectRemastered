using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
    public partial class User
    {
        public User()
        {
            Customers = new HashSet<Customer>();
            UserOperationClaims = new HashSet<UserOperationClaim>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;
        public byte[] PasswordHash { get; set; } = null!;
        public bool Status { get; set; }
        public int? FindeksScore { get; set; }
        public string? ProfileImagePath { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<UserOperationClaim> UserOperationClaims { get; set; }
    }
}
