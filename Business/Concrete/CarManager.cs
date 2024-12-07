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
        [CacheRemoveAspect("ICarService.Get")]
        public IResult Add(Car car)
        {
            try
            {
                _logger.LogInformation("Car Add Started: {@CarData}", new { 
                    car.BrandId, 
                    car.ColorId, 
                    car.ModelYear,
                    car.DailyPrice,
                    car.Description
                });

                // Validation
                var validator = new CarValidator();
                var validationResult = validator.Validate(car);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed: {@ValidationErrors}", 
                        validationResult.Errors.Select(e => e.ErrorMessage));
                    return new ErrorResult(validationResult.Errors.First().ErrorMessage);
                }

                // Clear navigation properties
                car.Brand = null;
                car.Color = null;
                car.CarImages = new List<CarImage>();

                // Set default values
                if (car.MinFindeksScore <= 0)
                {
                    car.MinFindeksScore = 500;
                }

                _carDal.Add(car);
                _logger.LogInformation("Car successfully added with ID: {CarId}", car.CarId);
                return new SuccessResult(Messages.CarAdded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding car: {@CarData}", new { 
                    car?.BrandId, 
                    car?.ColorId, 
                    car?.ModelYear,
                    car?.DailyPrice,
                    car?.Description
                });
                return new ErrorResult($"Araba eklenirken bir hata oluştu: {ex.Message}");
            }
        }

        [CacheRemoveAspect("ICarService.Get")]
        public IResult Delete(Car car)
        {
            _carDal.Delete(car);
            return new SuccessResult(Messages.CarDeleted);
        }

        [CacheRemoveAspect("ICarService.Get")]
        [ValidationAspect(typeof(CarValidator))]
        public IResult Update(Car car)
        {
            try
            {
                _logger.LogInformation("Car Update Started: {@CarData}", new { 
                    car.CarId,
                    car.BrandId, 
                    car.ColorId, 
                    car.ModelYear,
                    car.DailyPrice,
                    car.Description,
                    car.MinFindeksScore
                });

                // Validation
                var validator = new CarValidator();
                var validationResult = validator.Validate(car);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed: {@ValidationErrors}", 
                        validationResult.Errors.Select(e => e.ErrorMessage));
                    return new ErrorResult(validationResult.Errors.First().ErrorMessage);
                }

                // Arabanın var olup olmadığını kontrol et
                var existingCar = _carDal.Get(c => c.CarId == car.CarId);
                if (existingCar == null)
                {
                    _logger.LogWarning("Car not found: {CarId}", car.CarId);
                    return new ErrorResult("Güncellenecek araç bulunamadı");
                }

                // Navigation property'leri temizle
                car.Brand = null;
                car.Color = null;

                _carDal.Update(car);
                
                _logger.LogInformation("Car successfully updated: {@Car}", new {
                    car.CarId,
                    car.BrandId,
                    car.ColorId,
                    car.ModelYear,
                    car.DailyPrice,
                    car.Description,
                    car.MinFindeksScore
                });

                return new SuccessResult(Messages.CarUpdated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating car: {@CarData}", new { 
                    car?.CarId,
                    car?.BrandId, 
                    car?.ColorId, 
                    car?.ModelYear,
                    car?.DailyPrice,
                    car?.Description
                });
                return new ErrorResult($"Araç güncellenirken bir hata oluştu: {ex.Message}");
            }
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
