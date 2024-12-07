using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfCarImageDal : EfEntityRepositoryBase<CarImage,RentACarContext>,ICarImageDal
    {
        public override void Add(CarImage entity)
        {
            using (var context = new RentACarContext())
            {
                try
                {
                    var addedEntity = context.Entry(entity);
                    addedEntity.State = EntityState.Added;
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"=== Database Update Error ===");
                    Console.WriteLine($"Error: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }
                    throw;
                }
            }
        }
    }
}
