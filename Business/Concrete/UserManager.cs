using Business.Abstract;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using Core.Utilities.Security.Hashing;
using DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        IUserDal _userDal;

        public UserManager(IUserDal userDal)
        {
            _userDal = userDal;
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
                var user = _userDal.Get(u => u.Id == id);
                if (user == null)
                    return new ErrorDataResult<User>("Kullanıcı bulunamadı");

                return new SuccessDataResult<User>(user);
            }
            catch (Exception ex)
            {
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
    }
}
