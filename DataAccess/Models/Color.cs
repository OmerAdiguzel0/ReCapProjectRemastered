using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
    public partial class Color
    {
        public Color()
        {
            Cars = new HashSet<Car>();
        }

        public int ColorId { get; set; }
        public string ColorName { get; set; } = null!;

        public virtual ICollection<Car> Cars { get; set; }
    }
}
