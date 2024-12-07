using System;
using System.Collections.Generic;
using System.Linq;
using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfCarDal : EfEntityRepositoryBase<Car, RentACarContext>, ICarDal
    {
        public List<CarDetailDto> GetCarDetail()
        {
            using (RentACarContext context = new RentACarContext())
            {
                try
                {
                    Console.WriteLine("=== GetCarDetail Started ===");
                    
                    // Ana sorgu
                    var result = from c in context.Cars
                        join b in context.Brands
                            on c.BrandId equals b.BrandId
                        join co in context.Colors
                            on c.ColorId equals co.ColorId
                        select new CarDetailDto
                        {
                            CarId = c.CarId,
                            BrandName = b.BrandName,
                            ColorName = co.ColorName,
                            DailyPrice = c.DailyPrice,
                            ModelYear = c.ModelYear,
                            Description = c.Description,
                            MinFindeksScore = c.MinFindeksScore,
                            ImagePaths = context.CarImages
                                .Where(ci => ci.CarId == c.CarId)
                                .Select(ci => ci.ImagePath)
                                .ToList() ?? new List<string>()
                        };

                    var cars = result.ToList();
                    Console.WriteLine($"Found {cars.Count} cars");

                    // SQL sorgusunu logla
                    var sql = result.ToString();
                    Console.WriteLine($"Generated SQL: {sql}");

                    return cars;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"=== GetCarDetail Error ===");
                    Console.WriteLine($"Error Type: {ex.GetType().Name}");
                    Console.WriteLine($"Error Message: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }
                    throw;
                }
            }
        }

        public override void Add(Car entity)
        {
            using (var context = new RentACarContext())
            {
                var transaction = context.Database.BeginTransaction();
                try
                {
                    Console.WriteLine("=== EfCarDal Add Method Started ===");
                    Console.WriteLine($"Adding Car: BrandId={entity.BrandId}, " +
                                    $"ColorId={entity.ColorId}, " +
                                    $"ModelYear={entity.ModelYear}, " +
                                    $"DailyPrice={entity.DailyPrice}, " +
                                    $"Description={entity.Description}");

                    var addedEntity = context.Entry(entity);
                    addedEntity.State = EntityState.Added;

                    Console.WriteLine("SaveChanges öncesi...");
                    context.SaveChanges();
                    Console.WriteLine($"Eklenen arabanın ID'si: {entity.CarId}");
                    Console.WriteLine("SaveChanges sonrası...");

                    Console.WriteLine("Transaction commit öncesi...");
                    transaction.Commit();
                    Console.WriteLine("Transaction commit sonrası...");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("=== EfCarDal Add Method Error ===");
                    Console.WriteLine($"Error Type: {ex.GetType().Name}");
                    Console.WriteLine($"Error Message: {ex.Message}");
                    
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    transaction?.Dispose();
                }
            }
        }

        public RentACarContext GetContext()
        {
            return new RentACarContext();
        }
    }
}
