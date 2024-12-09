using Business.Abstract;
using Core.Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using System.Security.Claims;

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

        [HttpPost("changepassword")]
        [Authorize]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                // Mevcut kullanıcıyı al
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var userResult = _userService.GetByMail(userEmail);
                
                if (!userResult.Success)
                {
                    return BadRequest(new { success = false, message = "Kullanıcı bulunamadı" });
                }

                var user = userResult.Data;

                // Mevcut şifreyi doğrula
                if (!HashingHelper.VerifyPasswordHash(request.CurrentPassword, user.PasswordHash, user.PasswordSalt))
                {
                    return BadRequest(new { success = false, message = "Mevcut şifre yanlış" });
                }

                // Yeni şifre için hash oluştur
                byte[] passwordHash, passwordSalt;
                HashingHelper.CreatePasswordHash(request.NewPassword, out passwordHash, out passwordSalt);

                // Kullanıcının şifresini güncelle
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                var result = _userService.Update(user);

                if (result.Success)
                {
                    return Ok(new { success = true, message = "Şifre başarıyla güncellendi" });
                }

                return BadRequest(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Şifre değiştirme işlemi başarısız: {ex.Message}" });
            }
        }

        [HttpPost("profile-image")]
        [Authorize]
        public IActionResult UpdateProfileImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { success = false, message = "Dosya seçilmedi" });

                // Kullanıcı ID'sini token'dan al
                var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

                // Dosya uzantısını kontrol et
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                    return BadRequest(new { success = false, message = "Sadece .jpg, .jpeg ve .png dosyaları kabul edilir" });

                // Dosya boyutunu kontrol et (örn: max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                    return BadRequest(new { success = false, message = "Dosya boyutu 5MB'dan büyük olamaz" });

                // Dosyayı kaydet
                var fileName = $"profile_{userId}_{DateTime.Now.Ticks}{extension}";
                var path = Path.Combine("Uploads", "ProfileImages", fileName);
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // Veritabanını güncelle
                var result = _userService.UpdateProfileImage(userId, path);
                if (!result.Success)
                    return BadRequest(new { success = false, message = result.Message });

                return Ok(new { success = true, message = "Profil fotoğrafı güncellendi", data = path });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Profil fotoğrafı yüklenirken hata oluştu: {ex.Message}" });
            }
        }

        [HttpDelete("profile-image")]
        [Authorize]
        public IActionResult DeleteProfileImage()
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
                var result = _userService.DeleteProfileImage(userId);
                
                if (!result.Success)
                    return BadRequest(new { success = false, message = result.Message });

                return Ok(new { success = true, message = "Profil fotoğrafı silindi" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Profil fotoğrafı silinirken hata oluştu: {ex.Message}" });
            }
        }

        [HttpGet("profile-image")]
        [Authorize]
        public IActionResult GetProfileImage()
        {
            try
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
                var result = _userService.GetProfileImage(userId);
                
                if (!result.Success)
                    return BadRequest(new { success = false, message = result.Message });

                return Ok(new { success = true, data = result.Data });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Profil fotoğrafı alınırken hata oluştu: {ex.Message}" });
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

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
