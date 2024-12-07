using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarImagesController : ControllerBase
    {
        ICarImageService _carImageService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CarImagesController(ICarImageService carImageService, IWebHostEnvironment webHostEnvironment)
        {
            _carImageService = carImageService;
            _webHostEnvironment = webHostEnvironment;
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
                if (ImagePath == null)
                {
                    return BadRequest(new { success = false, message = "Dosya seçilmedi" });
                }

                // Dosya boyutu kontrolü (örn: 5MB)
                if (ImagePath.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { success = false, message = "Dosya boyutu çok büyük" });
                }

                // Dosya tipi kontrolü
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
                if (!allowedTypes.Contains(ImagePath.ContentType.ToLower()))
                {
                    return BadRequest(new { success = false, message = "Geçersiz dosya tipi" });
                }

                var result = _carImageService.Add(ImagePath, carId);
                if (result.Success)
                {
                    return Ok(new { success = true, message = result.Message });
                }

                return BadRequest(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
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
    }
}
