using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfModelDal : EfEntityRepositoryBase<Model, RentACarContext>, IModelDal
    {
        private readonly ILogger<EfModelDal> _logger;

        public EfModelDal(ILogger<EfModelDal> logger)
        {
            _logger = logger;
        }

        public RentACarContext GetContext()
        {
            return new RentACarContext();
        }

        public override void Add(Model entity)
        {
            try
            {
                using (var context = new RentACarContext())
                {
                    var entry = context.Entry(entity);
                    entry.State = EntityState.Added;
                    
                    _logger.LogInformation("Entity state set to Added");
                    _logger.LogInformation("Entity details: {@Entity}", entity);
                    _logger.LogInformation("Database connection state: {State}", 
                        context.Database.GetConnectionString());

                    context.SaveChanges();
                    _logger.LogInformation("Changes saved successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in EfModelDal.Add");
                _logger.LogError("Entity details at error: {@Entity}", entity);
                throw;
            }
        }
    }
} 