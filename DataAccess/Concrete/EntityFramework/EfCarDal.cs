using System;
using System.Collections.Generic;
using System.Linq;
using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfCarDal : EfEntityRepositoryBase<Car, RentACarContext>, ICarDal
    {
        private readonly ILogger<EfCarDal> _logger;

        public EfCarDal(ILogger<EfCarDal> logger = null)
        {
            _logger = logger ?? NullLogger<EfCarDal>.Instance;
        }

        public List<CarDetailDto> GetCarDetail()
        {
            using (RentACarContext context = new RentACarContext())
            {
                try
                {
                    Console.WriteLine("=== GetCarDetail Started ===");

                    var result = from c in context.Cars
                        .Include(c => c.Brand)
                        .Include(c => c.Color)
                        .Include(c => c.CarImages)
                        select new CarDetailDto
                        {
                            CarId = c.CarId,
                            BrandName = c.Brand.BrandName,
                            ColorName = c.Color.ColorName,
                            DailyPrice = c.DailyPrice,
                            ModelYear = c.ModelYear,
                            Description = c.Description,
                            MinFindeksScore = c.MinFindeksScore,
                            ImagePaths = c.CarImages.Any() 
                                ? c.CarImages.Select(ci => $"/Uploads/Images/{ci.ImagePath}").ToList()
                                : new List<string> { "/Uploads/Images/default.jpg" }
                        };

                    var cars = result.ToList();
                    Console.WriteLine($"Found {cars.Count} cars");
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
                    Console.WriteLine("\n=== EfCarDal Add Method Started ===");
                    Console.WriteLine("Gelen Araba Bilgileri:");
                    Console.WriteLine($"- BrandId: {entity.BrandId}");
                    Console.WriteLine($"- ColorId: {entity.ColorId}");
                    Console.WriteLine($"- ModelYear: {entity.ModelYear}");
                    Console.WriteLine($"- DailyPrice: {entity.DailyPrice}");
                    Console.WriteLine($"- Description: {entity.Description}");
                    Console.WriteLine($"- MinFindeksScore: {entity.MinFindeksScore}");

                    // Navigation property kontrolü
                    if (entity.Brand != null)
                        Console.WriteLine("Uyarı: Brand navigation property null olmalı");
                    if (entity.Color != null)
                        Console.WriteLine("Uyarı: Color navigation property null olmalı");
                    
                    Console.WriteLine($"CarImages Count: {entity.CarImages?.Count ?? 0}");
                    if (entity.CarImages?.Any() == true)
                    {
                        Console.WriteLine("CarImages içeriği:");
                        foreach (var image in entity.CarImages)
                        {
                            Console.WriteLine($"- ImageId: {image.Id}");
                            Console.WriteLine($"- ImagePath: {image.ImagePath}");
                            Console.WriteLine($"- Date: {image.Date}");
                        }
                    }

                    // Entity state'i ayarla
                    var addedEntity = context.Entry(entity);
                    Console.WriteLine($"Entity State: {addedEntity.State}");
                    addedEntity.State = EntityState.Added;
                    Console.WriteLine($"New Entity State: {addedEntity.State}");

                    // Değişiklikleri kaydet
                    Console.WriteLine("\nSaveChanges çağrılıyor...");
                    context.SaveChanges();
                    Console.WriteLine($"SaveChanges tamamlandı. Yeni CarId: {entity.CarId}");

                    // Transaction'ı commit et
                    Console.WriteLine("\nTransaction commit ediliyor...");
                    transaction.Commit();
                    Console.WriteLine("Transaction başarıyla commit edildi");
                    Console.WriteLine("=== EfCarDal Add Method Completed ===\n");
                }
                catch (DbUpdateException dbEx)
                {
                    Console.WriteLine("\n=== Database Update Error ===");
                    Console.WriteLine($"Error Type: {dbEx.GetType().Name}");
                    Console.WriteLine($"Error Message: {dbEx.Message}");
                    if (dbEx.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {dbEx.InnerException.Message}");
                        Console.WriteLine($"Stack Trace: {dbEx.StackTrace}");
                    }
                    
                    transaction.Rollback();
                    Console.WriteLine("Transaction rolled back");
                    throw new Exception("Veritabanı güncelleme hatası", dbEx);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n=== Unexpected Error ===");
                    Console.WriteLine($"Error Type: {ex.GetType().Name}");
                    Console.WriteLine($"Error Message: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                    }
                    
                    transaction.Rollback();
                    Console.WriteLine("Transaction rolled back");
                    throw new Exception("Beklenmeyen bir hata oluştu", ex);
                }
                finally
                {
                    transaction?.Dispose();
                }
            }
        }

        public override void Update(Car entity)
        {
            using (var context = new RentACarContext())
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    _logger.LogInformation("Updating car: {@Car}", entity);

                    // Mevcut arabayı ve resimlerini getir
                    var existingCar = context.Cars
                        .Include(c => c.CarImages)
                        .AsNoTracking()  // Entity tracking'i devre dışı bırak
                        .FirstOrDefault(c => c.CarId == entity.CarId);

                    if (existingCar == null)
                    {
                        throw new Exception($"CarId: {entity.CarId} olan araç bulunamadı");
                    }

                    // Mevcut resimleri sakla
                    var existingImages = existingCar.CarImages.ToList();

                    // Yeni entity'yi attach et
                    var entry = context.Entry(entity);
                    entry.State = EntityState.Modified;

                    // Navigation property'leri güncelleme
                    entity.Brand = null;
                    entity.Color = null;
                    entity.CarImages = existingImages;

                    // Değişiklikleri kaydet
                    context.SaveChanges();

                    // Transaction'ı commit et
                    transaction.Commit();

                    _logger.LogInformation("Car updated successfully: {@UpdatedCar}", new
                    {
                        entity.CarId,
                        entity.BrandId,
                        entity.ColorId,
                        entity.ModelYear,
                        entity.DailyPrice,
                        entity.Description,
                        entity.MinFindeksScore,
                        ImageCount = entity.CarImages?.Count ?? 0
                    });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency error while updating car: {CarId}", entity.CarId);
                    transaction.Rollback();
                    throw new Exception("Güncelleme sırasında çakışma oluştu. Lütfen tekrar deneyin.", ex);
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error while updating car: {CarId}", entity.CarId);
                    transaction.Rollback();
                    throw new Exception("Veritabanı güncelleme hatası oluştu.", ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating car: {CarId}", entity.CarId);
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public RentACarContext GetContext()
        {
            return new RentACarContext();
        }
    }
}
