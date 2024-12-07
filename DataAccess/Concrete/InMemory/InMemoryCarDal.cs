using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using DataAccess.Concrete.EntityFramework;

namespace DataAccess.Concrete.InMemory
{
    public class InMemoryCarDal:ICarDal
    {
        List<Car> _cars;
        List<Brand> _brands;
        List<Color> _colors;

        public InMemoryCarDal()
        {
            _cars = new List<Car>
            {
                new Car{CarId = 1,BrandId = 1,ColorId = 1,ModelYear = 2021,DailyPrice = 150,Description = "Hasarlı"},
                new Car{CarId = 2,BrandId = 2,ColorId = 2,ModelYear = 2022,DailyPrice = 200,Description = "Hasarsız"},
                new Car{CarId = 3,BrandId = 3,ColorId = 2,ModelYear = 2011,DailyPrice = 450,Description = "Hasarlı"},
                new Car{CarId = 4,BrandId = 2,ColorId = 3,ModelYear = 2021,DailyPrice = 300,Description = "Hasarsız"},
                new Car{CarId = 5,BrandId = 1,ColorId = 1,ModelYear = 2019,DailyPrice = 100,Description = "Hasarlı"}
            };

            _brands = new List<Brand>
            {
                new Brand{BrandId = 1, BrandName = "BMW"},
                new Brand{BrandId = 2, BrandName = "Mercedes"},
                new Brand{BrandId = 3, BrandName = "Audi"}
            };

            _colors = new List<Color>
            {
                new Color{ColorId = 1, ColorName = "Siyah"},
                new Color{ColorId = 2, ColorName = "Beyaz"},
                new Color{ColorId = 3, ColorName = "Kırmızı"}
            };
        }


        public List<Car> GetAll()
        {
            return _cars;
        }

        public List<Car> GetByBrandId(int brandId)
        {
            return _cars.Where(c=>c.BrandId==brandId).ToList();
        }

        public List<Car> GetByColorId(int colorId)
        {
            return _cars.Where(c=>c.ColorId==colorId).ToList();
        }

        public List<Car> GetAll(Expression<Func<Car, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        public Car Get(Expression<Func<Car, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public void Add(Car car)
        {
            _cars.Add(car);
        }

        public void Update(Car car)
        {
            Car carToUpdate = _cars.SingleOrDefault(c => c.CarId == car.CarId);
            carToUpdate.BrandId = car.BrandId;
            carToUpdate.ColorId = car.ColorId;
            carToUpdate.ModelYear = car.ModelYear;
            carToUpdate.DailyPrice = car.DailyPrice;
            carToUpdate.Description = car.Description;
        }

        public void Delete(Car car)
        {
            Car carToDelete = _cars.SingleOrDefault(c=>c.CarId==car.CarId);
            _cars.Remove(carToDelete);
        }

        public List<CarDetailDto> GetCarDetail()
        {
            var result = from car in _cars
                         join brand in _brands
                             on car.BrandId equals brand.BrandId
                         join color in _colors
                             on car.ColorId equals color.ColorId
                         select new CarDetailDto
                         {
                             CarId = car.CarId,
                             BrandName = brand.BrandName,
                             ColorName = color.ColorName,
                             DailyPrice = car.DailyPrice,
                             ModelYear = car.ModelYear,
                             Description = car.Description,
                             MinFindeksScore = car.MinFindeksScore,
                             ImagePaths = new List<string>()  // Boş liste oluştur
                         };
            return result.ToList();
        }

        public RentACarContext GetContext()
        {
            throw new NotImplementedException("InMemory implementation does not support database context");
        }
    }
}
