using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        ICarService _carService;
        public CarsController(ICarService carService)
        {
            _carService = carService;
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
                Console.WriteLine("\n=== CarsController Add Method Started ===");
                Console.WriteLine($"Gelen Car Verisi:");
                Console.WriteLine($"BrandId: {car?.BrandId}");
                Console.WriteLine($"ColorId: {car?.ColorId}");
                Console.WriteLine($"ModelYear: {car?.ModelYear}");
                Console.WriteLine($"DailyPrice: {car?.DailyPrice}");
                Console.WriteLine($"Description: {car?.Description}");
                Console.WriteLine($"MinFindeksScore: {car?.MinFindeksScore}");
                Console.WriteLine($"CarId: {car?.CarId}");

                var result = _carService.Add(car);
                
                Console.WriteLine("\n=== Add Result Details ===");
                Console.WriteLine($"Success: {result?.Success}");
                Console.WriteLine($"Message: {result?.Message}");
                
                if (result.Success)
                {
                    Console.WriteLine("\n=== Creating Response ===");
                    Console.WriteLine($"Car after Add:");
                    Console.WriteLine($"CarId: {car?.CarId}");
                    Console.WriteLine($"BrandId: {car?.BrandId}");
                    Console.WriteLine($"ColorId: {car?.ColorId}");
                    Console.WriteLine($"ModelYear: {car?.ModelYear}");
                    Console.WriteLine($"DailyPrice: {car?.DailyPrice}");
                    Console.WriteLine($"Description: {car?.Description}");
                    Console.WriteLine($"MinFindeksScore: {car?.MinFindeksScore}");

                    var response = new { 
                        success = true, 
                        message = result.Message,
                        data = new {
                            carId = car.CarId,
                            brandId = car.BrandId,
                            colorId = car.ColorId,
                            modelYear = car.ModelYear,
                            dailyPrice = car.DailyPrice,
                            description = car.Description,
                            minFindeksScore = car.MinFindeksScore
                        }
                    };

                    Console.WriteLine("\n=== Response Object ===");
                    Console.WriteLine($"Success: {response.success}");
                    Console.WriteLine($"Message: {response.message}");
                    Console.WriteLine($"Data.CarId: {response.data.carId}");
                    Console.WriteLine($"Data.BrandId: {response.data.brandId}");
                    Console.WriteLine($"Data.ColorId: {response.data.colorId}");

                    return Ok(response);
                }

                Console.WriteLine("\n=== Returning BadRequest ===");
                Console.WriteLine($"Error Message: {result.Message}");
                return BadRequest(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n=== CarsController Error Details ===");
                Console.WriteLine($"Error Type: {ex.GetType().Name}");
                Console.WriteLine($"Error Message: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception Type: {ex.InnerException.GetType().Name}");
                    Console.WriteLine($"Inner Exception Message: {ex.InnerException.Message}");
                }
                Console.WriteLine("\nStack Trace:");
                Console.WriteLine(ex.StackTrace);

                return BadRequest(new { success = false, message = "Araba eklenirken bir hata oluştu" });
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
