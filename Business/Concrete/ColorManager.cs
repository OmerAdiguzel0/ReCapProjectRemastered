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
    public class ColorManager : IColorService
    {
        private IColorDal _colorDal;
        private ICarDal _carDal;

        public ColorManager(IColorDal colorDal, ICarDal carDal)
        {
            _colorDal = colorDal;
            _carDal = carDal;
        }

        public IDataResult<List<Color>> GetAll()
        {
            return new SuccessDataResult<List<Color>>(_colorDal.GetAll());
        }

        public IDataResult<Color> GetById(int colorId)
        {
            return new SuccessDataResult<Color>(_colorDal.Get(c => c.ColorId == colorId));
        }

        [ValidationAspect(typeof(ColorValidator))]
        public IResult Add(Color color)
        {
            IResult result = BusinessRules.Run(CheckIfColorNameExists(color.ColorName));
            if (result != null)
            {
                return result;
            }

            _colorDal.Add(color);
            return new SuccessResult(Messages.ColorAdded);
        }

        public IResult Delete(Color color)
        {
            try
            {
                var carsWithColor = _carDal.GetAll(c => c.ColorId == color.ColorId);
                if (carsWithColor.Any())
                {
                    return new ErrorResult("Bu renge ait araçlar bulunduğu için silinemez");
                }

                _colorDal.Delete(color);
                return new SuccessResult(Messages.ColorDeleted);
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Renk silinirken bir hata oluştu: {ex.Message}");
            }
        }

        [ValidationAspect(typeof(ColorValidator))]
        public IResult Update(Color color)
        {
            var existingColor = _colorDal.Get(c => c.ColorId == color.ColorId);
            if (existingColor == null)
            {
                return new ErrorResult(Messages.ColorNotFound);
            }

            if (existingColor.ColorName != color.ColorName)
            {
                IResult result = BusinessRules.Run(CheckIfColorNameExists(color.ColorName));
                if (result != null)
                {
                    return result;
                }
            }

            _colorDal.Update(color);
            return new SuccessResult(Messages.ColorUpdated);
        }

        private IResult CheckIfColorNameExists(string colorName)
        {
            var result = _colorDal.GetAll(c => c.ColorName == colorName).Any();
            if (result)
            {
                return new ErrorResult(Messages.ColorNameAlreadyExists);
            }

            return new SuccessResult();
        }
    }
}
