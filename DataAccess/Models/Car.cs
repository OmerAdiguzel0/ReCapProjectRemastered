using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
    public partial class Car
    {
        public Car()
        {
            CarImages = new HashSet<CarImage>();
            Rentals = new HashSet<Rental>();
        }

        public int CarId { get; set; }
        public int BrandId { get; set; }
        public int ColorId { get; set; }
        public int ModelYear { get; set; }
        public int DailyPrice { get; set; }
        public string Description { get; set; } = null!;
        public int MinFindeksScore { get; set; }

        public virtual Brand Brand { get; set; } = null!;
        public virtual Color Color { get; set; } = null!;
        public virtual ICollection<CarImage> CarImages { get; set; }
        public virtual ICollection<Rental> Rentals { get; set; }
    }
}
