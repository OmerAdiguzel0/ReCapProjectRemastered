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

        public void AddOperationClaim(OperationClaim operationClaim)
        {
            using (var context = new RentACarContext())
            {
                var addedEntity = context.Entry(operationClaim);
                addedEntity.State = Microsoft.EntityFrameworkCore.EntityState.Added;
                context.SaveChanges();
            }
        }

        public void DeleteOperationClaim(OperationClaim operationClaim)
        {
            using (var context = new RentACarContext())
            {
                var deletedEntity = context.Entry(operationClaim);
                deletedEntity.State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                context.SaveChanges();
            }
        }

        public void UpdateOperationClaim(OperationClaim operationClaim)
        {
            using (var context = new RentACarContext())
            {
                var updatedEntity = context.Entry(operationClaim);
                updatedEntity.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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

        public void UpdateUserOperationClaim(int userId, int roleId)
        {
            using (var context = new RentACarContext())
            {
                // Önce kullanıcının mevcut rolünü bul
                var existingClaim = context.UserOperationClaims
                    .FirstOrDefault(uoc => uoc.UserId == userId);

                if (existingClaim != null)
                {
                    // Mevcut rolü güncelle
                    existingClaim.OperationClaimId = roleId;
                    var updatedEntity = context.Entry(existingClaim);
                    updatedEntity.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                else
                {
                    // Yeni rol ata
                    var newClaim = new UserOperationClaim
                    {
                        UserId = userId,
                        OperationClaimId = roleId
                    };
                    var addedEntity = context.Entry(newClaim);
                    addedEntity.State = Microsoft.EntityFrameworkCore.EntityState.Added;
                }

                context.SaveChanges();
            }
        }

        public List<OperationClaim> GetOperationClaims()
        {
            using (var context = new RentACarContext())
            {
                return context.OperationClaims.ToList();
            }
        }

        public void DeleteUserOperationClaims(int userId)
        {
            using (var context = new RentACarContext())
            {
                var claims = context.UserOperationClaims.Where(uoc => uoc.UserId == userId);
                context.UserOperationClaims.RemoveRange(claims);
                context.SaveChanges();
            }
        }
    }
}
