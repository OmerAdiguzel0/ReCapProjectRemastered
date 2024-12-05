﻿﻿using System;
using System.Collections.Generic;
using Core.Entities;

namespace Entities.Concrete
{
    public class Car : IEntity
    {
        public Car()
        {
            CarImages = new List<CarImage>();
        }

        public int CarId { get; set; }
        public int BrandId { get; set; }
        public int ColorId { get; set; }
        public int ModelYear { get; set; }
        public int DailyPrice { get; set; }
        public string Description { get; set; }
        public int MinFindeksScore { get; set; } = 500;

        // Navigation properties
        public Brand Brand { get; set; }
        public Color Color { get; set; }
        public ICollection<CarImage> CarImages { get; set; }
    }
}
