using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly ICarService _carService;
        private readonly ILogger<CarsController> _logger;

        public CarsController(ICarService carService, ILogger<CarsController> logger)
        {
            _carService = carService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _carService.GetAll();
            if (result.Success)
            {
                return Ok(new { success = true, data = result.Data });
            }

            return BadRequest(new { success = false, message = result.Message });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _carService.GetById(id);
            if (result.Success)
            {
                return Ok(new { success = true, data = result.Data });
            }

            return BadRequest(new { success = false, message = result.Message });
        }

        [HttpGet("brand/{id}")]
        public IActionResult GetByBrandId(int id)
        {
            var result = _carService.GetCarsByBrandId(id);
            if (result.Success)
            {
                return Ok(new { success = true, data = result.Data });
            }

            return BadRequest(new { success = false, message = result.Message });
        }
        
        [HttpGet("color/{id}")]
        public IActionResult GetByColorId(int id)
        {
            var result = _carService.GetCarsByColorId(id);
            if (result.Success)
            {
                return Ok(new { success = true, data = result.Data });
            }

            return BadRequest(new { success = false, message = result.Message });
        }
        
        [HttpGet("detail")]
        public IActionResult GetDetail()
        {
            var result = _carService.GetCarDetail();
            if (result.Success)
            {
                return Ok(new { success = true, data = result.Data });
            }

            return BadRequest(new { success = false, message = result.Message });
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Add([FromBody] Car car)
        {
            try
            {
                _logger.LogInformation("\n===========================================");
                _logger.LogInformation("Car Add Request Started at {Time}", DateTime.Now);
                _logger.LogInformation("Request Details:");
                _logger.LogInformation("User: {User}", User.Identity.Name);
                _logger.LogInformation("Car Data: {@CarData}", new
                {
                    car?.BrandId,
                    car?.ColorId,
                    car?.ModelYear,
                    car?.DailyPrice,
                    car?.Description
                });

                // Request validation
                if (car == null)
                {
                    _logger.LogWarning("Request body is null");
                    return BadRequest(new { success = false, message = "Geçersiz istek: Araba bilgileri boş" });
                }

                // Log incoming data
                _logger.LogInformation("Incoming Car Data: {@Car}", new 
                { 
                    car.BrandId, 
                    car.ColorId, 
                    car.ModelYear,
                    car.DailyPrice,
                    car.Description,
                    car.MinFindeksScore
                });

                // Basic validation
                if (car.BrandId <= 0 || car.ColorId <= 0)
                {
                    _logger.LogWarning("Invalid brand or color ID: Brand={BrandId}, Color={ColorId}", car.BrandId, car.ColorId);
                    return BadRequest(new { success = false, message = "Geçersiz marka veya renk seçimi" });
                }

                if (car.DailyPrice <= 0)
                {
                    _logger.LogWarning("Invalid daily price: {Price}", car.DailyPrice);
                    return BadRequest(new { success = false, message = "Günlük fiyat 0'dan büyük olmalıdır" });
                }

                // Call service
                _logger.LogInformation("Calling CarService.Add");
                var result = _carService.Add(car);
                
                // Log service result
                _logger.LogInformation("Service Response: Success={Success}, Message={Message}", 
                    result.Success, result.Message);

                if (result.Success)
                {
                    var response = new { success = true, message = result.Message, data = car };
                    _logger.LogInformation("Success Response: {@Response}", response);
                    return Ok(response);
                }

                _logger.LogWarning("Service returned failure: {Message}", result.Message);
                return BadRequest(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("\n===========================================");
                _logger.LogError("Error occurred at {Time}", DateTime.Now);
                _logger.LogError("Error Type: {ErrorType}", ex.GetType().Name);
                _logger.LogError("Error Message: {ErrorMessage}", ex.Message);
                _logger.LogError("Stack Trace: {StackTrace}", ex.StackTrace);
                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner Exception: {InnerError}", ex.InnerException.Message);
                }
                _logger.LogError("===========================================\n");

                return StatusCode(500, new { success = false, message = "Araba eklenirken bir hata oluştu", error = ex.Message });
            }
            finally
            {
                _logger.LogInformation("Request completed at {Time}", DateTime.Now);
                _logger.LogInformation("===========================================\n");
            }
        }
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            var carResult = _carService.GetById(id);
            if (!carResult.Success)
            {
                return BadRequest(new { success = false, message = carResult.Message });
            }

            var result = _carService.Delete(carResult.Data);
            if (result.Success)
            {
                return Ok(new { success = true, message = result.Message });
            }

            return BadRequest(new { success = false, message = result.Message });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Update(int id, Car car)
        {
            car.CarId = id;
            var result = _carService.Update(car);
            if (result.Success)
            {
                return Ok(new { success = true, data = car, message = result.Message });
            }

            return BadRequest(new { success = false, message = result.Message });
        }
    }
}
