using Core.DataAccess;
using Core.Entities.Concrete;
using System.Collections.Generic;

namespace DataAccess.Abstract
{
    public interface IUserDal : IEntityRepository<User>
    {
        List<OperationClaim> GetClaims(User user);
        void AddOperationClaim(OperationClaim operationClaim);
        void DeleteOperationClaim(OperationClaim operationClaim);
        void UpdateOperationClaim(OperationClaim operationClaim);
        void AddUserOperationClaim(UserOperationClaim userOperationClaim);
        void UpdateUserOperationClaim(int userId, int roleId);
        void DeleteUserOperationClaims(int userId);
        List<OperationClaim> GetOperationClaims();
    }
}
