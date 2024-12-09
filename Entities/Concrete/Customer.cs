using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace Entities.Concrete
{
    public class Customer : IEntity
    {
        public int CustomerId { get; set; }  // Primary Key
        public int UserId { get; set; }      // Foreign Key to Users table
        public string CompanyName { get; set; }
    }
}
