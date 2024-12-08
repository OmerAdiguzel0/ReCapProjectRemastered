using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Performance;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Validation.FluentValidation;
using Core.Utilities.Business;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;

namespace Business.Concrete
{
    public class CarManager : ICarService
    {
        private ICarDal _carDal;
        private readonly ILogger<CarManager> _logger;

        public CarManager(ICarDal carDal, ILogger<CarManager> logger)
        {
            _carDal = carDal;
            _logger = logger;
        }

        [CacheAspect]
        public IDataResult<List<Car>> GetAll()
        {
            return new SuccessDataResult<List<Car>>(_carDal.GetAll(), Messages.CarsListed);
        }

        public IDataResult<List<Car>> GetCarsByBrandId(int id)
        {
            return new SuccessDataResult<List<Car>>(_carDal.GetAll(c => c.BrandId == id));
        }

        public IDataResult<List<Car>> GetCarsByColorId(int id)
        {
            return new SuccessDataResult<List<Car>>(_carDal.GetAll(c => c.ColorId == id));
        }

        public IDataResult<List<CarDetailDto>> GetCarDetail()
        {
            return new SuccessDataResult<List<CarDetailDto>>(_carDal.GetCarDetail());
        }

        [CacheAspect]
        public IDataResult<Car> GetById(int carId)
        {
            return new SuccessDataResult<Car>(_carDal.Get(c => c.CarId == carId));
        }

        [SecuredOperation("car.add,admin")]
        [ValidationAspect(typeof(CarValidator))]
        [LogAspect(typeof(FileLogger))]
        [CacheRemoveAspect("ICarService.Get")]
        public IResult Add(Car car)
        {
            // Navigation property'leri temizle
            car.Brand = null;
            car.Color = null;
            car.CarImages = new List<CarImage>();

            // Default findeks puanını ayarla
            if (car.MinFindeksScore <= 0)
            {
                car.MinFindeksScore = 500;
            }

            _carDal.Add(car);
            return new SuccessResult(Messages.CarAdded);
        }

        [SecuredOperation("car.delete,admin")]
        [LogAspect(typeof(FileLogger))]
        [CacheRemoveAspect("ICarService.Get")]
        public IResult Delete(Car car)
        {
            var existingCar = _carDal.Get(c => c.CarId == car.CarId);
            if (existingCar == null)
            {
                return new ErrorResult(Messages.CarNotFound);
            }

            _carDal.Delete(car);
            return new SuccessResult(Messages.CarDeleted);
        }

        [SecuredOperation("car.update,admin")]
        [ValidationAspect(typeof(CarValidator))]
        [LogAspect(typeof(FileLogger))]
        [CacheRemoveAspect("ICarService.Get")]
        public IResult Update(Car car)
        {
            var existingCar = _carDal.Get(c => c.CarId == car.CarId);
            if (existingCar == null)
            {
                return new ErrorResult(Messages.CarNotFound);
            }

            // Navigation property'leri temizle
            car.Brand = null;
            car.Color = null;

            _carDal.Update(car);
            return new SuccessResult(Messages.CarUpdated);
        }

        private IResult CheckInCarCountOfBrandCorrect(int brandId)
        {
            const int MAX_CARS_PER_BRAND = 20;

            var result = _carDal.GetAll(c => c.BrandId == brandId).Count;
            if (result >= MAX_CARS_PER_BRAND)
            {
                _logger.LogWarning($"Brand {brandId} has reached maximum car limit of {MAX_CARS_PER_BRAND}");
                return new ErrorResult($"Bir markaya en fazla {MAX_CARS_PER_BRAND} araç eklenebilir!");
            }

            return new SuccessResult();
        }

        [TransactionScopeAspect]
        public IResult AddTransactionalTest(Car car)
        {
            _carDal.Update(car);
            _carDal.Add(car);
            return new SuccessResult(Messages.CarUpdated);
        }
    }
}
