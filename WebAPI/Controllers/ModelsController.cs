using Business.Abstract;
using Business.ValidationRules.FluentValidation;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelsController : ControllerBase
    {
        private IModelService _modelService;
        private readonly ILogger<ModelsController> _logger;

        public ModelsController(IModelService modelService, ILogger<ModelsController> logger)
        {
            _modelService = modelService;
            _logger = logger;
        }

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            try
            {
                _logger.LogInformation("GetAll Models request received");
                var result = _modelService.GetAll();
                
                if (result.Success)
                {
                    _logger.LogInformation("GetAll Models request successful");
                    return Ok(new { success = true, data = result.Data });
                }
                
                _logger.LogWarning("GetAll Models request failed: {Message}", result.Message);
                return BadRequest(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAll Models");
                return BadRequest(new { success = false, message = "Model verileri alınırken bir hata oluştu: " + ex.Message });
            }
        }

        [HttpGet("getbybrand/{brandId}")]
        public IActionResult GetByBrand(int brandId)
        {
            try
            {
                var result = _modelService.GetByBrandId(brandId);
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

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _modelService.GetById(id);
            if (result.Success)
            {
                return Ok(new { success = true, data = result.Data });
            }
            return BadRequest(new { success = false, message = result.Message });
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody] Model model)
        {
            try
            {
                _logger.LogInformation("=== Model Add Request Started ===");
                _logger.LogInformation("Received model data: {@Model}", model);
                
                if (model == null)
                {
                    _logger.LogWarning("Model data is null");
                    return BadRequest(new { success = false, message = "Model verisi boş olamaz" });
                }

                var validator = new ModelValidator();
                var validationResult = validator.Validate(model);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed: {@ValidationErrors}", 
                        validationResult.Errors.Select(e => new { Property = e.PropertyName, Error = e.ErrorMessage }));
                    return BadRequest(new { 
                        success = false, 
                        message = "Validasyon hatası",
                        errors = validationResult.Errors.Select(e => e.ErrorMessage)
                    });
                }

                _logger.LogInformation("Attempting to add model to database...");
                var result = _modelService.Add(model);
                
                if (result.Success)
                {
                    _logger.LogInformation("Model successfully added with ID: {ModelId}", model.ModelId);
                    return Ok(new { success = true, message = result.Message });
                }

                _logger.LogWarning("Failed to add model: {Message}", result.Message);
                return BadRequest(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "=== Unhandled Error in Model Add Request ===");
                _logger.LogError("Exception Type: {ExceptionType}", ex.GetType().Name);
                _logger.LogError("Exception Message: {Message}", ex.Message);
                
                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner Exception: {InnerMessage}", ex.InnerException.Message);
                    _logger.LogError("Inner Stack Trace: {InnerStackTrace}", ex.InnerException.StackTrace);
                }
                
                return BadRequest(new { 
                    success = false, 
                    message = "Model eklenirken bir hata oluştu: " + ex.Message,
                    details = ex.InnerException?.Message
                });
            }
        }

        [HttpPut("update")]
        public IActionResult Update(Model model)
        {
            var result = _modelService.Update(model);
            if (result.Success)
            {
                return Ok(new { success = true, message = result.Message });
            }
            return BadRequest(new { success = false, message = result.Message });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var model = _modelService.GetById(id);
            if (!model.Success)
            {
                return BadRequest(new { success = false, message = "Model bulunamadı" });
            }

            var result = _modelService.Delete(model.Data);
            if (result.Success)
            {
                return Ok(new { success = true, message = result.Message });
            }
            return BadRequest(new { success = false, message = result.Message });
        }
    }
} 