using Business.Abstract;
using Business.Constants;
using Core.Utilities.Business;
using Core.Utilities.Helpers.FileHelper;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;

namespace Business.Concrete
{
    public class CarImageManager : ICarImageService
    {
        ICarImageDal _carImageDal;
        IFileHelper _fileHelper;

        public CarImageManager(ICarImageDal carImageDal, IFileHelper fileHelper)
        {
            _carImageDal = carImageDal;
            _fileHelper = fileHelper;
        }

        public IResult Add(IFormFile formFile, CarImage carImage)
        {
            try 
            {
                if (formFile == null)
                {
                    return new ErrorResult("Dosya seçilmedi");
                }

                IResult result = BusinessRules.Run(CheckIfCarImageCountOfCarCorrect(carImage.CarId));
                if (result != null)
                {
                    return result;
                }

                string uploadedFileName = _fileHelper.Upload(formFile, PathConstants.ImagesPath);
                if (string.IsNullOrEmpty(uploadedFileName))
                {
                    return new ErrorResult("Dosya yüklenemedi");
                }

                carImage.ImagePath = uploadedFileName;
                carImage.Date = DateTime.UtcNow;
                _carImageDal.Add(carImage);

                return new SuccessResult(Messages.ImageUploaded);
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Hata oluştu: {ex.Message}");
            }
        }

        public IResult Delete(CarImage carImage)
        {
            try
            {
                if (carImage == null || string.IsNullOrEmpty(carImage.ImagePath))
                {
                    return new ErrorResult("Geçersiz resim bilgisi");
                }

                string fullPath = Path.Combine(PathConstants.ImagesPath, carImage.ImagePath);
                _fileHelper.Delete(fullPath);
                _carImageDal.Delete(carImage);
                return new SuccessResult(Messages.DeletedImage);
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Silme işlemi sırasında hata oluştu: {ex.Message}");
            }
        }

        public IDataResult<List<CarImage>> GetAll()
        {
            return new SuccessDataResult<List<CarImage>>(_carImageDal.GetAll());
        }

        public IDataResult<List<CarImage>> GetByCarId(int carId)
        {
            var result = BusinessRules.Run(CheckCarImage(carId));
            if (result != null)
            {
                return new ErrorDataResult<List<CarImage>>(GetDefaultImage(carId).Data);
            }

            var images = _carImageDal.GetAll(c => c.CarId == carId);
            foreach (var image in images)
            {
                // API'nin base URL'sine göre resim yolunu güncelle
                image.ImagePath = $"/Uploads/Images/{image.ImagePath}";
            }

            return new SuccessDataResult<List<CarImage>>(images);
        }

        public IDataResult<CarImage> GetByImageId(int carImageId)
        {
            var image = _carImageDal.Get(c => c.CarId == carImageId);
            if (image != null)
            {
                // API'nin base URL'sine göre resim yolunu güncelle
                image.ImagePath = $"/Uploads/Images/{image.ImagePath}";
            }
            return new SuccessDataResult<CarImage>(image);
        }

        public IResult Update(IFormFile formFile, CarImage carImage)
        {
            try
            {
                if (formFile == null)
                {
                    return new ErrorResult("Dosya seçilmedi");
                }

                if (carImage == null || string.IsNullOrEmpty(carImage.ImagePath))
                {
                    return new ErrorResult("Geçersiz resim bilgisi");
                }

                string oldPath = Path.Combine(PathConstants.ImagesPath, carImage.ImagePath);
                string newFileName = _fileHelper.Update(formFile, oldPath, PathConstants.ImagesPath);
                
                if (string.IsNullOrEmpty(newFileName))
                {
                    return new ErrorResult("Dosya güncellenemedi");
                }

                carImage.ImagePath = newFileName;
                carImage.Date = DateTime.UtcNow;
                _carImageDal.Update(carImage);
                return new SuccessResult(Messages.UpdatedImage);
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Güncelleme işlemi sırasında hata oluştu: {ex.Message}");
            }
        }

        private IResult CheckIfCarImageCountOfCarCorrect(int carId)
        {
            var result = _carImageDal.GetAll(c => c.CarId == carId).Count;
            if (result >= 5)
            {
                return new ErrorResult(Messages.CarImageCountOfCarError);
            }
            return new SuccessResult();
        }

        private IDataResult<List<CarImage>> GetDefaultImage(int carId)
        {
            List<CarImage> carImage = new List<CarImage>();
            carImage.Add(new CarImage { CarId = carId, Date = DateTime.UtcNow, ImagePath = "/Uploads/Images/Default.jpg" });
            return new SuccessDataResult<List<CarImage>>(carImage);
        }

        private IResult CheckCarImage(int carId)
        {
            var result = _carImageDal.GetAll(c => c.CarId == carId).Any();
            if (result)
            {
                return new SuccessResult();
            }
            return new ErrorResult();
        }
    }
}
