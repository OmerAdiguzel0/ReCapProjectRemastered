using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
    public partial class OperationClaim
    {
        public OperationClaim()
        {
            UserOperationClaims = new HashSet<UserOperationClaim>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<UserOperationClaim> UserOperationClaims { get; set; }
    }
}
