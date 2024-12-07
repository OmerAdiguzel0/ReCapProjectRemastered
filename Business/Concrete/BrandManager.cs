using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.Utilities.Business;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class BrandManager:IBrandService
    {
        private IBrandDal _brandDal;
        private ICarDal _carDal;

        public BrandManager(IBrandDal brandDal, ICarDal carDal)
        {
            _brandDal = brandDal;
            _carDal = carDal;
        }

        public IDataResult<List<Brand>> GetAll()
        {
            return new SuccessDataResult<List<Brand>>(_brandDal.GetAll());
        }

        public IDataResult<Brand> GetById(int brandId)
        {
            return new SuccessDataResult<Brand>(_brandDal.Get(b => b.BrandId == brandId));
        }

        [ValidationAspect(typeof(BrandValidator))]
        public IResult Add(Brand brand)
        {
            IResult result = BusinessRules.Run(CheckIfBrandNameExists(brand.BrandName));
            if (result != null)
            {
                return result;
            }

            _brandDal.Add(brand);
            return new SuccessResult(Messages.BrandAdded);
        }

        public IResult Delete(Brand brand)
        {
            try
            {
                // Marka ile ilişkili araçları kontrol et
                var carsWithBrand = _carDal.GetAll(c => c.BrandId == brand.BrandId);
                if (carsWithBrand.Any())
                {
                    return new ErrorResult("Bu markaya ait araçlar bulunduğu için silinemez");
                }

                _brandDal.Delete(brand);
                return new SuccessResult(Messages.BrandDeleted);
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Marka silinirken bir hata oluştu: {ex.Message}");
            }
        }

        [ValidationAspect(typeof(BrandValidator))]
        public IResult Update(Brand brand)
        {
            var existingBrand = _brandDal.Get(b => b.BrandId == brand.BrandId);
            if (existingBrand == null)
            {
                return new ErrorResult(Messages.BrandNotFound);
            }

            if (existingBrand.BrandName != brand.BrandName)
            {
                IResult result = BusinessRules.Run(CheckIfBrandNameExists(brand.BrandName));
                if (result != null)
                {
                    return result;
                }
            }

            _brandDal.Update(brand);
            return new SuccessResult(Messages.BrandUpdated);
        }

        private IResult CheckIfBrandNameExists(string brandName)
        {
            var result = _brandDal.GetAll(b => b.BrandName == brandName).Any();
            if (result)
            {
                return new ErrorResult(Messages.BrandNameAlreadyExists);
            }

            return new SuccessResult();
        }
    }
}
