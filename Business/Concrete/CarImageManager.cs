using Business.Abstract;
using Business.Constants;
using Core.Utilities.Business;
using Core.Utilities.Helpers.FileHelper;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Business.Concrete
{
    public class CarImageManager : ICarImageService
    {
        ICarImageDal _carImageDal;
        IFileHelper _fileHelper;
        ICarService _carService;
        ILogger<CarImageManager> _logger;

        public CarImageManager(ICarImageDal carImageDal, IFileHelper fileHelper, ICarService carService, ILogger<CarImageManager> logger)
        {
            _carImageDal = carImageDal;
            _fileHelper = fileHelper;
            _carService = carService;
            _logger = logger;
        }

        public IResult Add(IFormFile file, CarImage carImage)
        {
            try
            {
                _logger.LogInformation("Adding image for car {CarId}", carImage.CarId);

                IResult result = BusinessRules.Run(CheckIfCarImageCountOfCarCorrect(carImage.CarId));
                if (result != null)
                {
                    _logger.LogWarning("Business rule check failed: {Message}", result.Message);
                    return result;
                }

                var uploadResult = _fileHelper.Upload(file, PathConstants.ImagesPath);
                if (string.IsNullOrEmpty(uploadResult))
                {
                    _logger.LogError("File upload failed");
                    return new ErrorResult("Dosya yüklenemedi");
                }

                carImage.ImagePath = uploadResult;
                carImage.Date = DateTime.UtcNow;

                _carImageDal.Add(carImage);

                _logger.LogInformation("Image successfully added: {@CarImage}", new {
                    carImage.Id,
                    carImage.CarId,
                    carImage.ImagePath,
                    carImage.Date
                });

                return new SuccessResult(Messages.ImageUploaded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding car image");
                return new ErrorResult($"Resim eklenirken bir hata oluştu: {ex.Message}");
            }
        }

        public IResult Delete(CarImage carImage)
        {
            try
            {
                if (carImage == null)
                {
                    return new ErrorResult("Geçersiz resim bilgisi");
                }

                // Eğer resim default resim ise silmeye çalışma
                if (carImage.ImagePath != null && carImage.ImagePath.Contains("default.jpg"))
                {
                    _carImageDal.Delete(carImage);
                    return new SuccessResult(Messages.DeletedImage);
                }

                // Resim yolu boş değilse ve dosya varsa sil
                if (!string.IsNullOrEmpty(carImage.ImagePath))
                {
                    string fullPath = Path.Combine(PathConstants.ImagesPath, carImage.ImagePath);
                    if (System.IO.File.Exists(fullPath))
                    {
                        _fileHelper.Delete(fullPath);
                    }
                }

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
            try
            {
                var result = _carImageDal.GetAll(c => c.CarId == carId);
                if (!result.Any())
                {
                    return new SuccessDataResult<List<CarImage>>(new List<CarImage>
                    {
                        new CarImage
                        {
                            CarId = carId,
                            ImagePath = "default.jpg",
                            Date = DateTime.Now
                        }
                    });
                }

                // Resimleri API URL'sine göre düzenle
                foreach (var image in result)
                {
                    if (!string.IsNullOrEmpty(image.ImagePath))
                    {
                        image.ImagePath = $"/Uploads/Images/{image.ImagePath}";
                    }
                }

                return new SuccessDataResult<List<CarImage>>(result);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<CarImage>>(ex.Message);
            }
        }

        public IDataResult<CarImage> GetByImageId(int imageId)
        {
            try
            {
                var result = _carImageDal.Get(c => c.Id == imageId);
                if (result == null)
                {
                    return new ErrorDataResult<CarImage>("Resim bulunamadı");
                }

                return new SuccessDataResult<CarImage>(result);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<CarImage>($"Resim getirilirken bir hata oluştu: {ex.Message}");
            }
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
