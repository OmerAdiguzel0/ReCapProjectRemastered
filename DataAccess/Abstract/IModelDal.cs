using Core.DataAccess;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;

namespace DataAccess.Abstract
{
    public interface IModelDal : IEntityRepository<Model>
    {
        RentACarContext GetContext();
    }
} 