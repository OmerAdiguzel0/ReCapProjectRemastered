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

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var result = _carImageService.GetAll();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        
        [HttpGet("getbycarid")]
        public IActionResult GetByCarId(int carId)
        {
            var result = _carImageService.GetByCarId(carId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        
        [HttpGet("getbyimageid")]
        public IActionResult GetByImageId(int imageId)
        {
            var result = _carImageService.GetByCarId(imageId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("add")]
        public IActionResult Add([FromForm(Name = "ImagePath")] IFormFile formFile, [FromForm] CarImage carImage)
        {
            try
            {
                if (formFile == null)
                {
                    return BadRequest(new { success = false, message = "Dosya seçilmedi" });
                }

                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", "Images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var result = _carImageService.Add(formFile, carImage);
                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }
        }

        [HttpPost("delete")]
        public IActionResult Delete([FromBody] CarImage carImage)
        {
            try
            {
                if (carImage == null)
                {
                    return BadRequest(new { success = false, message = "Geçersiz resim bilgisi" });
                }

                var result = _carImageService.Delete(carImage);
                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }
        }

        [HttpPost("update")]
        public IActionResult Update([FromForm] IFormFile formFile, [FromForm] CarImage carImage)
        {
            try
            {
                if (formFile == null)
                {
                    return BadRequest(new { success = false, message = "Dosya seçilmedi" });
                }

                if (carImage == null)
                {
                    return BadRequest(new { success = false, message = "Geçersiz resim bilgisi" });
                }

                var result = _carImageService.Update(formFile, carImage);
                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }
        }
    }
}
