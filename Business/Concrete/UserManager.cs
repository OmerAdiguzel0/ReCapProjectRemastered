using Business.Abstract;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using Core.Utilities.Security.Hashing;
using DataAccess.Abstract;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        private IUserDal _userDal;
        private readonly ILogger<UserManager> _logger;

        public UserManager(IUserDal userDal, ILogger<UserManager> logger)
        {
            _userDal = userDal;
            _logger = logger;
        }

        public IResult Add(User user)
        {
            try
            {
                _userDal.Add(user);
                AddDefaultRole(user); // Yeni kullanıcıya default rol ata
                return new SuccessResult("Kullanıcı eklendi");
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Kullanıcı eklenirken hata oluştu: {ex.Message}");
            }
        }

        public IResult AddDefaultRole(User user)
        {
            try
            {
                var defaultRole = _userDal.GetOperationClaims().FirstOrDefault(c => c.Name == "user");
                if (defaultRole == null)
                {
                    defaultRole = new OperationClaim { Name = "user" };
                    _userDal.AddOperationClaim(defaultRole);
                }

                _userDal.AddUserOperationClaim(new UserOperationClaim
                {
                    UserId = user.Id,
                    OperationClaimId = defaultRole.Id
                });

                return new SuccessResult();
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Default rol eklenirken hata oluştu: {ex.Message}");
            }
        }

        public IResult Delete(User user)
        {
            try
            {
                // Kullanıcının rollerini kontrol et
                var claims = GetClaims(user);
                if (!claims.Success)
                {
                    return new ErrorResult("Kullanıcı rolleri alınamadı");
                }

                // Admin kullanıcısının silinmesini engelle
                if (claims.Data.Any(c => c.Name == "admin"))
                {
                    return new ErrorResult("Admin kullanıcısı silinemez");
                }

                // Önce kullanıcının rollerini sil
                _userDal.DeleteUserOperationClaims(user.Id);

                // Sonra kullanıcıyı sil
                _userDal.Delete(user);
                return new SuccessResult(Messages.DeletedUser);
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Kullanıcı silinirken bir hata oluştu: {ex.Message}");
            }
        }

        public IDataResult<List<User>> GetAll()
        {
            try
            {
                return new SuccessDataResult<List<User>>(_userDal.GetAll());
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<User>>($"Kullanıcılar listelenirken hata oluştu: {ex.Message}");
            }
        }

        public IDataResult<User> GetByMail(string email)
        {
            try
            {
                var user = _userDal.Get(u => u.Email == email);
                if (user == null)
                    return new ErrorDataResult<User>("Kullanıcı bulunamadı");

                return new SuccessDataResult<User>(user);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<User>($"Kullanıcı aranırken hata oluştu: {ex.Message}");
            }
        }

        public IDataResult<List<OperationClaim>> GetClaims(User user)
        {
            try
            {
                return new SuccessDataResult<List<OperationClaim>>(_userDal.GetClaims(user));
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<OperationClaim>>($"Kullanıcı yetkileri alınırken hata oluştu: {ex.Message}");
            }
        }

        public IResult Update(User user)
        {
            try
            {
                _userDal.Update(user);
                return new SuccessResult();
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Kullanıcı güncellenirken hata oluştu: {ex.Message}");
            }
        }

        public IResult UpdateUserRole(int userId, int roleId)
        {
            try
            {
                var user = _userDal.Get(u => u.Id == userId);
                if (user == null)
                    return new ErrorResult("Kullanıcı bulunamadı");

                var operationClaim = _userDal.GetOperationClaims().FirstOrDefault(c => c.Id == roleId);
                if (operationClaim == null)
                    return new ErrorResult("Rol bulunamadı");

                // Yeni metodu kullanarak rolü güncelle
                _userDal.UpdateUserOperationClaim(userId, roleId);

                return new SuccessResult("Kullanıcı rolü güncellendi");
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Kullanıcı rolü güncellenirken hata oluştu: {ex.Message}");
            }
        }

        public IDataResult<List<OperationClaim>> GetAllRoles()
        {
            try
            {
                var roles = _userDal.GetOperationClaims();
                return new SuccessDataResult<List<OperationClaim>>(roles);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<OperationClaim>>($"Roller listelenirken hata oluştu: {ex.Message}");
            }
        }

        public IResult AddRole(string roleName)
        {
            try
            {
                if (_userDal.GetOperationClaims().Any(r => r.Name == roleName))
                {
                    return new ErrorResult("Bu rol zaten mevcut");
                }

                var newRole = new OperationClaim { Name = roleName };
                _userDal.AddOperationClaim(newRole);
                return new SuccessResult("Rol başarıyla eklendi");
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Rol eklenirken hata oluştu: {ex.Message}");
            }
        }

        public IResult DeleteRole(int roleId)
        {
            try
            {
                var role = _userDal.GetOperationClaims().FirstOrDefault(r => r.Id == roleId);
                if (role == null)
                {
                    return new ErrorResult("Rol bulunamadı");
                }

                if (role.Name == "admin" || role.Name == "user")
                {
                    return new ErrorResult("Varsayılan roller silinemez");
                }

                _userDal.DeleteOperationClaim(role);
                return new SuccessResult("Rol başarıyla silindi");
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Rol silinirken hata oluştu: {ex.Message}");
            }
        }

        public IResult UpdateRole(int roleId, string roleName)
        {
            try
            {
                var role = _userDal.GetOperationClaims().FirstOrDefault(r => r.Id == roleId);
                if (role == null)
                {
                    return new ErrorResult("Rol bulunamadı");
                }

                if (role.Name == "admin" || role.Name == "user")
                {
                    return new ErrorResult("Varsayılan roller güncellenemez");
                }

                if (_userDal.GetOperationClaims().Any(r => r.Name == roleName && r.Id != roleId))
                {
                    return new ErrorResult("Bu rol adı zaten kullanılıyor");
                }

                role.Name = roleName;
                _userDal.UpdateOperationClaim(role);
                return new SuccessResult("Rol başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Rol güncellenirken hata oluştu: {ex.Message}");
            }
        }

        public IDataResult<User> GetById(int id)
        {
            try
            {
                Console.WriteLine($"\n=== UserManager.GetById Started ===");
                Console.WriteLine($"Searching for User ID: {id}");

                var user = _userDal.Get(u => u.Id == id);
                
                if (user == null)
                {
                    Console.WriteLine("User not found");
                    return new ErrorDataResult<User>("Kullanıcı bulunamadı");
                }

                Console.WriteLine($"User found: ID={user.Id}, Email={user.Email}, FindeksScore={user.FindeksScore}");
                Console.WriteLine("=== UserManager.GetById Completed ===\n");

                return new SuccessDataResult<User>(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n=== UserManager.GetById Error ===");
                Console.WriteLine($"Error Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine("=== Error Log End ===\n");

                return new ErrorDataResult<User>($"Kullanıcı aranırken hata oluştu: {ex.Message}");
            }
        }

        public IResult ChangePassword(User user, string currentPassword, string newPassword)
        {
            try
            {
                // Mevcut şifreyi doğrula
                if (!HashingHelper.VerifyPasswordHash(currentPassword, user.PasswordHash, user.PasswordSalt))
                {
                    return new ErrorResult("Mevcut şifre yanlış");
                }

                // Yeni şifre için hash oluştur
                byte[] passwordHash, passwordSalt;
                HashingHelper.CreatePasswordHash(newPassword, out passwordHash, out passwordSalt);

                // Kullanıcının şifresini güncelle
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                _userDal.Update(user);
                return new SuccessResult("Şifre başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Şifre değiştirme işlemi başarısız: {ex.Message}");
            }
        }

        public IResult UpdateProfileImage(int userId, string imagePath)
        {
            try
            {
                var user = _userDal.Get(u => u.Id == userId);
                if (user == null)
                    return new ErrorResult("Kullanıcı bulunamadı");

                // Eski resmi sil
                if (!string.IsNullOrEmpty(user.ProfileImagePath))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfileImagePath);
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }

                user.ProfileImagePath = imagePath;
                _userDal.Update(user);
                return new SuccessResult("Profil fotoğrafı güncellendi");
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Profil fotoğrafı güncellenirken hata oluştu: {ex.Message}");
            }
        }

        public IResult DeleteProfileImage(int userId)
        {
            try
            {
                var user = _userDal.Get(u => u.Id == userId);
                if (user == null)
                    return new ErrorResult("Kullanıcı bulunamadı");

                if (!string.IsNullOrEmpty(user.ProfileImagePath))
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfileImagePath);
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }

                user.ProfileImagePath = null;
                _userDal.Update(user);
                return new SuccessResult("Profil fotoğrafı silindi");
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Profil fotoğrafı silinirken hata oluştu: {ex.Message}");
            }
        }

        public IDataResult<string> GetProfileImage(int userId)
        {
            try
            {
                var user = _userDal.Get(u => u.Id == userId);
                if (user == null)
                    return new ErrorDataResult<string>("Kullanıcı bulunamadı");

                return new SuccessDataResult<string>(user.ProfileImagePath);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<string>($"Profil fotoğrafı alınırken hata oluştu: {ex.Message}");
            }
        }

        public IResult UpdateFindeksScore(int userId, int newScore)
        {
            try
            {
                var user = _userDal.Get(u => u.Id == userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", userId);
                    return new ErrorResult("Kullanıcı bulunamadı");
                }

                if (newScore < 0 || newScore > 1900)
                {
                    _logger.LogWarning("Invalid findeks score: {Score} for user {UserId}", newScore, userId);
                    return new ErrorResult("Findeks puanı 0-1900 arasında olmalıdır");
                }

                user.FindeksScore = newScore;
                _userDal.Update(user);

                _logger.LogInformation("Findeks score updated for user {UserId}: {Score}", userId, newScore);
                return new SuccessResult("Findeks puanı başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating findeks score for user {UserId}", userId);
                return new ErrorResult($"Findeks puanı güncellenirken bir hata oluştu: {ex.Message}");
            }
        }
    }
}
