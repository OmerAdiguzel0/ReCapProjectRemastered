using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Abstract;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private IAuthService _authService;
        private IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("login")]
        public ActionResult Login(UserForLoginDto userForLoginDto)
        {
            var userToLogin = _authService.Login(userForLoginDto);
            if (!userToLogin.Success)
            {
                return BadRequest(new { success = false, message = userToLogin.Message });
            }

            var result = _authService.CreateAccessToken(userToLogin.Data);
            if (result.Success)
            {
                // Kullanıcı rollerini kontrol et
                var claims = _userService.GetClaims(userToLogin.Data);
                bool isAdmin = claims.Data.Any(c => c.Name == "admin");

                var response = new
                {
                    success = true,
                    message = "Giriş başarılı",
                    data = new
                    {
                        token = result.Data.Token,
                        expiration = result.Data.Expiration,
                        email = userToLogin.Data.Email,
                        userId = userToLogin.Data.Id,
                        isAdmin = isAdmin
                    }
                };
                return Ok(response);
            }

            return BadRequest(new { success = false, message = result.Message });
        }

        [HttpPost("admin/login")]
        public ActionResult AdminLogin(UserForLoginDto userForLoginDto)
        {
            var userToLogin = _authService.Login(userForLoginDto);
            if (!userToLogin.Success)
            {
                return BadRequest(new { success = false, message = userToLogin.Message });
            }

            // Admin rolünü kontrol et
            var claims = _userService.GetClaims(userToLogin.Data);
            bool isAdmin = claims.Data.Any(c => c.Name == "admin");

            if (!isAdmin)
            {
                return BadRequest(new { success = false, message = "Bu işlem için yetkiniz yok" });
            }

            var result = _authService.CreateAccessToken(userToLogin.Data);
            if (result.Success)
            {
                var response = new
                {
                    success = true,
                    message = "Admin girişi başarılı",
                    data = new
                    {
                        token = result.Data.Token,
                        expiration = result.Data.Expiration,
                        email = userToLogin.Data.Email,
                        userId = userToLogin.Data.Id,
                        isAdmin = true
                    }
                };
                return Ok(response);
            }

            return BadRequest(new { success = false, message = result.Message });
        }

        [HttpPost("register")]
        public ActionResult Register(UserForRegisterDto userForRegisterDto)
        {
            // Temel validasyonlar
            if (userForRegisterDto == null)
            {
                return BadRequest(new { success = false, message = "Kayıt bilgileri boş olamaz" });
            }

            if (string.IsNullOrEmpty(userForRegisterDto.Email))
            {
                return BadRequest(new { success = false, message = "Email adresi boş olamaz" });
            }

            if (string.IsNullOrEmpty(userForRegisterDto.Password))
            {
                return BadRequest(new { success = false, message = "Şifre boş olamaz" });
            }

            // Email kontrolü
            var userExists = _authService.UserExists(userForRegisterDto.Email);
            if (!userExists.Success)
            {
                return BadRequest(new { success = false, message = userExists.Message });
            }

            // Kayıt işlemi
            var registerResult = _authService.Register(userForRegisterDto, userForRegisterDto.Password);
            if (!registerResult.Success)
            {
                // Validasyon hatası veya diğer hatalar
                return BadRequest(new { success = false, message = registerResult.Message });
            }

            // Token oluşturma
            var result = _authService.CreateAccessToken(registerResult.Data);
            if (!result.Success)
            {
                // Token oluşturma hatası - kullanıcı kaydedildi ama token oluşturulamadı
                return BadRequest(new { success = false, message = result.Message });
            }

            // Başarılı kayıt
            var response = new
            {
                success = true,
                message = "Kayıt başarılı",
                data = new
                {
                    token = result.Data.Token,
                    expiration = result.Data.Expiration,
                    email = registerResult.Data.Email,
                    userId = registerResult.Data.Id
                }
            };
            return Ok(response);
        }
    }
}
