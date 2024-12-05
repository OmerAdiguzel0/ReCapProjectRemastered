using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using System.Collections.Generic;

namespace Business.Abstract
{
    public interface IUserService
    {
        IDataResult<List<OperationClaim>> GetClaims(User user);
        IDataResult<List<User>> GetAll();
        IDataResult<User> GetByMail(string email);
        IResult Add(User user);
        IResult Update(User user);
        IResult Delete(User user);
        IResult UpdateUserRole(int userId, int roleId);
        IResult AddDefaultRole(User user);
        
        // Rol yönetimi metodları
        IDataResult<List<OperationClaim>> GetAllRoles();
        IResult AddRole(string roleName);
        IResult DeleteRole(int roleId);
        IResult UpdateRole(int roleId, string roleName);
    }
}
