using Business.Abstract;
using Core.Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Utilities.Results;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("getall")]
        [Authorize(Roles = "admin")]
        public IActionResult GetAll()
        {
            var result = _userService.GetAll();
            if (result.Success)
            {
                var users = result.Data.Select(user => new
                {
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.Status,
                    RoleId = _userService.GetClaims(user).Data.FirstOrDefault()?.Id,
                    RoleName = _userService.GetClaims(user).Data.FirstOrDefault()?.Name
                });

                return Ok(new { success = true, data = users });
            }
            return BadRequest(new { success = false, message = result.Message });
        }

        [HttpGet("roles")]
        [Authorize(Roles = "admin")]
        public IActionResult GetAllRoles()
        {
            var result = _userService.GetAllRoles();
            if (result.Success)
            {
                return Ok(new { success = true, data = result.Data });
            }
            return BadRequest(new { success = false, message = result.Message });
        }

        [HttpPost("roles")]
        [Authorize(Roles = "admin")]
        public IActionResult AddRole([FromBody] RoleRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { success = false, message = "Rol adı boş olamaz" });
            }

            var result = _userService.AddRole(request.Name);
            if (result.Success)
            {
                return Ok(new { success = true, message = result.Message });
            }
            return BadRequest(new { success = false, message = result.Message });
        }

        [HttpPut("roles/{roleId}")]
        [Authorize(Roles = "admin")]
        public IActionResult UpdateRole(int roleId, [FromBody] RoleRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { success = false, message = "Rol adı boş olamaz" });
            }

            var result = _userService.UpdateRole(roleId, request.Name);
            if (result.Success)
            {
                return Ok(new { success = true, message = result.Message });
            }
            return BadRequest(new { success = false, message = result.Message });
        }

        [HttpDelete("roles/{roleId}")]
        [Authorize(Roles = "admin")]
        public IActionResult DeleteRole(int roleId)
        {
            var result = _userService.DeleteRole(roleId);
            if (result.Success)
            {
                return Ok(new { success = true, message = result.Message });
            }
            return BadRequest(new { success = false, message = result.Message });
        }

        [HttpPut("role/{userId}")]
        [Authorize(Roles = "admin")]
        public IActionResult UpdateUserRole(int userId, [FromBody] RoleUpdateRequest request)
        {
            if (request.RoleId <= 0)
            {
                return BadRequest(new { success = false, message = "Geçersiz rol ID" });
            }

            var result = _userService.UpdateUserRole(userId, request.RoleId);
            if (result.Success)
            {
                return Ok(new { success = true, message = "Kullanıcı rolü güncellendi" });
            }
            return BadRequest(new { success = false, message = result.Message });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var userToDelete = _userService.GetById(id);
                if (!userToDelete.Success)
                {
                    return BadRequest(new { success = false, message = "Kullanıcı bulunamadı" });
                }

                // Admin kullanıcısının silinmesini engelle
                var userClaims = _userService.GetClaims(userToDelete.Data);
                if (userClaims.Data.Any(c => c.Name == "admin"))
                {
                    return BadRequest(new { success = false, message = "Admin kullanıcısı silinemez" });
                }

                var result = _userService.Delete(userToDelete.Data);
                if (result.Success)
                {
                    return Ok(new { success = true, message = result.Message });
                }

                return BadRequest(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Silme işlemi sırasında hata oluştu: {ex.Message}" });
            }
        }
    }

    public class RoleRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    public class RoleUpdateRequest
    {
        public int RoleId { get; set; }
    }
}
