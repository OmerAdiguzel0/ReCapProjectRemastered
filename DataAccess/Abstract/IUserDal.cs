using Core.DataAccess;
using Core.Entities.Concrete;
using System.Collections.Generic;

namespace DataAccess.Abstract
{
    public interface IUserDal : IEntityRepository<User>
    {
        List<OperationClaim> GetClaims(User user);
        List<OperationClaim> GetOperationClaims();
        void AddOperationClaim(OperationClaim operationClaim);
        void AddUserOperationClaim(UserOperationClaim userOperationClaim);
        void DeleteUserOperationClaims(int userId);
        void DeleteOperationClaim(OperationClaim operationClaim); // Yeni eklenen metod
    }
}
