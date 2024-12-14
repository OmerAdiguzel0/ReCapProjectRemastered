using Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FindeksController : ControllerBase
    {
        private readonly IFindeksService _findeksService;
        private readonly ILogger<FindeksController> _logger;

        public FindeksController(IFindeksService findeksService, ILogger<FindeksController> logger)
        {
            _findeksService = findeksService;
            _logger = logger;
        }

        [HttpPost("calculate")]
        public IActionResult Calculate([FromBody] FindeksCalculateRequest request)
        {
            try
            {
                _logger.LogInformation("\n=== Findeks Calculate Request Started ===");
                _logger.LogInformation($"Request Data: {JsonSerializer.Serialize(request)}");

                if (request == null)
                {
                    _logger.LogError("Request object is null");
                    return BadRequest(new { success = false, message = "Geçersiz istek" });
                }

                if (string.IsNullOrEmpty(request.TCKN))
                {
                    _logger.LogError("TCKN is null or empty");
                    return BadRequest(new { success = false, message = "TCKN boş olamaz" });
                }

                if (string.IsNullOrEmpty(request.BirthYear))
                {
                    _logger.LogError("BirthYear is null or empty");
                    return BadRequest(new { success = false, message = "Doğum yılı boş olamaz" });
                }

                _logger.LogInformation($"Calling FindeksService.CalculateFindeksScore with TCKN: {request.TCKN.Substring(0, 3)}***{request.TCKN.Substring(7)}, BirthYear: {request.BirthYear}");

                var result = _findeksService.CalculateFindeksScore(request.TCKN, request.BirthYear);

                if (result.Success)
                {
                    _logger.LogInformation($"Calculation successful. Score: {result.Data}");
                    return Ok(new { success = true, data = result.Data, message = result.Message });
                }

                _logger.LogWarning($"Calculation failed: {result.Message}");
                return BadRequest(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Findeks calculation error");
                _logger.LogError($"Request Data: {JsonSerializer.Serialize(request)}");
                _logger.LogError($"Stack Trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner Exception: {ex.InnerException.Message}");
                    _logger.LogError($"Inner Exception Stack Trace: {ex.InnerException.StackTrace}");
                }

                return BadRequest(new { success = false, message = "Findeks hesaplanırken bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost("update/{userId}")]
        public IActionResult UpdateFindeksScore(int userId, [FromBody] int findeksScore)
        {
            try
            {
                _logger.LogInformation("\n=== Update Findeks Score Request Started ===");
                _logger.LogInformation($"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                _logger.LogInformation($"User ID: {userId}");
                _logger.LogInformation($"New Score: {findeksScore}");

                var result = _findeksService.UpdateUserFindeksScore(userId, findeksScore);

                if (result.Success)
                {
                    _logger.LogInformation("Update successful");
                    _logger.LogInformation("=== Update Findeks Score Request Completed ===\n");
                    return Ok(new { success = true, message = result.Message });
                }

                _logger.LogWarning($"Update failed: {result.Message}");
                _logger.LogInformation("=== Update Findeks Score Request Failed ===\n");
                return BadRequest(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("\n=== Update Findeks Score Request Error ===");
                _logger.LogError($"Error Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                _logger.LogError($"Error Type: {ex.GetType().Name}");
                _logger.LogError($"Error Message: {ex.Message}");
                _logger.LogError($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner Exception: {ex.InnerException.Message}");
                }
                _logger.LogError("=== Error Log End ===\n");

                return BadRequest(new { success = false, message = "Findeks puanı güncellenirken bir hata oluştu" });
            }
        }
    }

    public class FindeksCalculateRequest
    {
        public string TCKN { get; set; }
        public string BirthYear { get; set; }
    }
} 