using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Business.Constants;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarImagesController : ControllerBase
    {
        private readonly ICarImageService _carImageService;
        private readonly ILogger<CarImagesController> _logger;

        public CarImagesController(ICarImageService carImageService, ILogger<CarImagesController> logger)
        {
            _carImageService = carImageService;
            _logger = logger;
        }

        [HttpGet("getbycarid")]
        public IActionResult GetByCarId(int carId)
        {
            try
            {
                if (carId <= 0)
                {
                    return BadRequest(new { success = false, message = "Geçersiz araba ID'si" });
                }

                var result = _carImageService.GetByCarId(carId);
                if (result.Success)
                {
                    return Ok(new { success = true, data = result.Data });
                }

                return BadRequest(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }
        }

        [HttpPost("add")]
        public IActionResult Add([FromForm] IFormFile ImagePath, [FromForm] int carId)
        {
            try
            {
                _logger.LogInformation("=== CarImage Add Request Started ===");
                _logger.LogInformation("Request Details: CarId={CarId}, FileName={FileName}, FileSize={FileSize}", 
                    carId, ImagePath?.FileName, ImagePath?.Length);

                if (ImagePath == null || ImagePath.Length == 0)
                {
                    _logger.LogWarning("No file uploaded or file is empty");
                    return BadRequest(new { success = false, message = "Dosya seçilmedi veya boş" });
                }

                // Dosya uzantısı kontrolü
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(ImagePath.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    _logger.LogWarning("Invalid file extension: {Extension}", extension);
                    return BadRequest(new { success = false, message = "Sadece .jpg, .jpeg ve .png dosyaları kabul edilir" });
                }

                // Dosya boyutu kontrolü (örn: 5MB)
                if (ImagePath.Length > 5 * 1024 * 1024)
                {
                    _logger.LogWarning("File too large: {Size}bytes", ImagePath.Length);
                    return BadRequest(new { success = false, message = "Dosya boyutu 5MB'dan büyük olamaz" });
                }

                // CarImage nesnesini oluştur
                var carImage = new CarImage
                {
                    CarId = carId,
                    Date = DateTime.Now
                };

                var result = _carImageService.Add(ImagePath, carImage);
                _logger.LogInformation("Service Response: {@Result}", result);

                if (result.Success)
                {
                    return Ok(new { success = true, message = result.Message, data = carImage });
                }

                _logger.LogWarning("Service returned failure: {Message}", result.Message);
                return BadRequest(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image for car {CarId}", carId);
                return BadRequest(new { 
                    success = false, 
                    message = "Resim yükleme sırasında bir hata oluştu",
                    error = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var carImage = _carImageService.GetByImageId(id).Data;
                if (carImage == null)
                {
                    return BadRequest(new { success = false, message = "Resim bulunamadı" });
                }

                var result = _carImageService.Delete(carImage);
                if (result.Success)
                {
                    return Ok(new { success = true, message = result.Message });
                }

                return BadRequest(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }
        }

        [HttpDelete("deleteByPath")]
        public IActionResult DeleteByPath([FromBody] DeleteImageRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.ImagePath))
                {
                    return BadRequest(new { success = false, message = "Resim yolu boş olamaz" });
                }

                // Dosya yolundan sadece dosya adını al
                string fileName = Path.GetFileName(request.ImagePath);

                // Veritabanından resmi bul
                var carImage = _carImageService.GetAll().Data
                    .FirstOrDefault(ci => ci.ImagePath == fileName);

                if (carImage == null)
                {
                    return NotFound(new { success = false, message = "Resim kaydı bulunamadı" });
                }

                // Fiziksel dosyayı sil
                string fullPath = Path.Combine(PathConstants.ImagesPath, fileName);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                // Veritabanından resmi sil
                var result = _carImageService.Delete(carImage);
                if (result.Success)
                {
                    return Ok(new { success = true, message = "Resim başarıyla silindi" });
                }

                return BadRequest(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Resim silinirken bir hata oluştu: {ex.Message}" });
            }
        }

        public class DeleteImageRequest
        {
            public string ImagePath { get; set; }
        }
    }
}
