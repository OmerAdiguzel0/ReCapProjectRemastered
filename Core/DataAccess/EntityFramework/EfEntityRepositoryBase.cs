using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.DataAccess.EntityFramework
{
    public class EfEntityRepositoryBase<TEntity,TContext>: IEntityRepository<TEntity>
    where TEntity : class,IEntity,new()
    where TContext:DbContext,new()
    {
        public List<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null)
        {
            try
            {
                using (TContext context = new TContext())
                {
                    return filter == null
                        ? context.Set<TEntity>().ToList()
                        : context.Set<TEntity>().Where(filter).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Veri getirme işlemi sırasında bir hata oluştu: {ex.Message}");
            }
        }

        public TEntity Get(Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                using (TContext context = new TContext())
                {
                    return context.Set<TEntity>().SingleOrDefault(filter);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Veri getirme işlemi sırasında bir hata oluştu: {ex.Message}");
            }
        }

        public void Add(TEntity entity)
        {
            try
            {
                using (TContext context = new TContext())
                {
                    var addEntity = context.Entry(entity);
                    addEntity.State = EntityState.Added;
                    context.SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    if (innerException.Message.Contains("duplicate key"))
                    {
                        throw new Exception("Bu kayıt zaten mevcut.");
                    }
                    innerException = innerException.InnerException;
                }
                throw new Exception($"Veritabanı ekleme işlemi sırasında bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ekleme işlemi sırasında bir hata oluştu: {ex.Message}");
            }
        }

        public void Update(TEntity entity)
        {
            try
            {
                using (TContext context = new TContext())
                {
                    var updateEntity = context.Entry(entity);
                    updateEntity.State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Güncelleme işlemi sırasında bir hata oluştu: {ex.Message}");
            }
        }

        public void Delete(TEntity entity)
        {
            try
            {
                using (TContext context = new TContext())
                {
                    var deleteEntity = context.Entry(entity);
                    deleteEntity.State = EntityState.Deleted;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Silme işlemi sırasında bir hata oluştu: {ex.Message}");
            }
        }
    }
}
