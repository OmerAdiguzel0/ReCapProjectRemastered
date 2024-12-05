using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.JWT;
using Entities.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Concrete
{
     public class AuthManager : IAuthService
    {
        private IUserService _userService;
        private ITokenHelper _tokenHelper;

        public AuthManager(IUserService userService, ITokenHelper tokenHelper)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
        }

        [ValidationAspect(typeof(UserForRegisterDtoValidator))]
        public IDataResult<User> Register(UserForRegisterDto userForRegisterDto, string password)
        {
            // Validasyon kontrolü try bloğu dışında yapılıyor
            var validator = new UserForRegisterDtoValidator();
            var validationResult = validator.Validate(userForRegisterDto);
            if (!validationResult.IsValid)
            {
                return new ErrorDataResult<User>(null, validationResult.Errors[0].ErrorMessage);
            }

            try
            {
                // Şifre hash'leme
                byte[] passwordHash, passwordSalt;
                HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);
                
                var user = new User
                {
                    Email = userForRegisterDto.Email,
                    FirstName = userForRegisterDto.FirstName,
                    LastName = userForRegisterDto.LastName,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Status = true,
                    FindeksScore = null // Başlangıçta null olarak ayarlandı
                };

                // Kullanıcı ekleme
                var addResult = _userService.Add(user);
                if (!addResult.Success)
                {
                    return new ErrorDataResult<User>(null, addResult.Message);
                }

                return new SuccessDataResult<User>(user, Messages.UserRegistered);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<User>(null, "Kayıt işlemi sırasında bir hata oluştu: " + ex.Message);
            }
        }

        public IDataResult<User> Login(UserForLoginDto userForLoginDto)
        {
            try
            {
                var userToCheck = _userService.GetByMail(userForLoginDto.Email);
                if (!userToCheck.Success || userToCheck.Data == null)
                {
                    return new ErrorDataResult<User>("E-posta adresi veya şifre hatalı");
                }

                if (!HashingHelper.VerifyPasswordHash(userForLoginDto.Password, userToCheck.Data.PasswordHash, userToCheck.Data.PasswordSalt))
                {
                    return new ErrorDataResult<User>("E-posta adresi veya şifre hatalı");
                }

                if (!userToCheck.Data.Status)
                {
                    return new ErrorDataResult<User>("Hesabınız aktif değil");
                }

                return new SuccessDataResult<User>(userToCheck.Data, Messages.SuccessfulLogin);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<User>("Giriş işlemi sırasında bir hata oluştu: " + ex.Message);
            }
        }

        public IResult UserExists(string email)
        {
            try
            {
                var userToCheck = _userService.GetByMail(email);
                if (userToCheck.Success && userToCheck.Data != null)
                {
                    return new ErrorResult("Bu e-posta adresi zaten kullanılıyor");
                }
                return new SuccessResult();
            }
            catch (Exception ex)
            {
                return new ErrorResult("Kullanıcı kontrolü sırasında bir hata oluştu: " + ex.Message);
            }
        }

        public IDataResult<AccessToken> CreateAccessToken(User user)
        {
            try
            {
                var claims = _userService.GetClaims(user);
                if (!claims.Success)
                {
                    return new ErrorDataResult<AccessToken>(claims.Message);
                }

                var accessToken = _tokenHelper.CreateToken(user, claims.Data);
                return new SuccessDataResult<AccessToken>(accessToken, Messages.AccessTokenCreated);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<AccessToken>("Token oluşturulurken bir hata oluştu: " + ex.Message);
            }
        }
    }
}
