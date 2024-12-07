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

namespace Business.Concrete
{
    public class CarManager : ICarService
    {
        private ICarDal _carDal;

        public CarManager(ICarDal carDal)
        {
            _carDal = carDal;
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
                Console.WriteLine("=== CarManager Add Method Started ===");
                
                if (car == null)
                    return new ErrorResult("Araba bilgileri boş olamaz");

                Console.WriteLine($"Validations Started - BrandId: {car.BrandId}, ColorId: {car.ColorId}");

                if (car.BrandId <= 0)
                    return new ErrorResult($"Geçersiz marka ID: {car.BrandId}");
                    
                if (car.ColorId <= 0)
                    return new ErrorResult($"Geçersiz renk ID: {car.ColorId}");

                if (string.IsNullOrWhiteSpace(car.Description))
                    return new ErrorResult("Açıklama boş olamaz");

                Console.WriteLine("Navigation properties cleaning...");
                car.Brand = null;
                car.Color = null;
                car.CarImages ??= new List<CarImage>();

                Console.WriteLine("Business rules check started...");
                IResult result = BusinessRules.Run(CheckInCarCountOfBrandCorrect(car.BrandId));
                if (result != null)
                {
                    Console.WriteLine($"Business rule failed: {result.Message}");
                    return result;
                }

                Console.WriteLine("Calling EfCarDal.Add...");
                try
                {
                    _carDal.Add(car);
                    Console.WriteLine("Car successfully added");
                    return new SuccessResult(Messages.CarAdded);
                }
                catch (Exception innerEx)
                {
                    Console.WriteLine($"=== Error in EfCarDal.Add ===");
                    Console.WriteLine($"Error Type: {innerEx.GetType().Name}");
                    Console.WriteLine($"Error Message: {innerEx.Message}");
                    if (innerEx.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {innerEx.InnerException.Message}");
                    }
                    return new ErrorResult("Araba eklenirken bir hata oluştu");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== CarManager Add Method Error ===");
                Console.WriteLine($"Error Type: {ex.GetType().Name}");
                Console.WriteLine($"Error Message: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
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
            IResult result = BusinessRules.Run(CheckInCarCountOfBrandCorrect(car.BrandId));
            if (result != null)
            {
                return result;
            }

            _carDal.Update(car);
            return new SuccessResult(Messages.CarUpdated);
        }

        private IResult CheckInCarCountOfBrandCorrect(int brandId)
        {
            var result = _carDal.GetAll(c=>c.BrandId == brandId).Count;
            if (result >= 10)
            {
                return new ErrorResult(Messages.CarCountOfBrandError);
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
