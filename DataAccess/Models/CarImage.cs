using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
    public partial class CarImage
    {
        public int CarImageId { get; set; }
        public int CarId { get; set; }
        public string? ImagePath { get; set; }
        public DateTime Date { get; set; }

        public virtual Car Car { get; set; } = null!;
    }
}
