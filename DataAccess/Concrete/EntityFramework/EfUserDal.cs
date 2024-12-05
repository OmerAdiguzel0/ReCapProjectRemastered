using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfUserDal : EfEntityRepositoryBase<User, RentACarContext>, IUserDal
    {
        public List<OperationClaim> GetClaims(User user)
        {
            using (var context = new RentACarContext())
            {
                var result = from operationClaim in context.OperationClaims
                            join userOperationClaim in context.UserOperationClaims
                            on operationClaim.Id equals userOperationClaim.OperationClaimId
                            where userOperationClaim.UserId == user.Id
                            select new OperationClaim { Id = operationClaim.Id, Name = operationClaim.Name };
                return result.ToList();
            }
        }

        public List<OperationClaim> GetOperationClaims()
        {
            using (var context = new RentACarContext())
            {
                return context.OperationClaims.ToList();
            }
        }

        public void AddOperationClaim(OperationClaim operationClaim)
        {
            using (var context = new RentACarContext())
            {
                var addedEntity = context.Entry(operationClaim);
                addedEntity.State = Microsoft.EntityFrameworkCore.EntityState.Added;
                context.SaveChanges();
            }
        }

        public void AddUserOperationClaim(UserOperationClaim userOperationClaim)
        {
            using (var context = new RentACarContext())
            {
                var addedEntity = context.Entry(userOperationClaim);
                addedEntity.State = Microsoft.EntityFrameworkCore.EntityState.Added;
                context.SaveChanges();
            }
        }

        public void DeleteUserOperationClaims(int userId)
        {
            using (var context = new RentACarContext())
            {
                var claims = context.UserOperationClaims.Where(u => u.UserId == userId);
                context.UserOperationClaims.RemoveRange(claims);
                context.SaveChanges();
            }
        }

        public void DeleteOperationClaim(OperationClaim operationClaim)
        {
            using (var context = new RentACarContext())
            {
                // Önce bu role sahip tüm kullanıcıların rol bağlantılarını sil
                var userClaims = context.UserOperationClaims
                    .Where(uc => uc.OperationClaimId == operationClaim.Id);
                context.UserOperationClaims.RemoveRange(userClaims);

                // Sonra rolü sil
                var entity = context.Entry(operationClaim);
                entity.State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                context.SaveChanges();
            }
        }
    }
}
