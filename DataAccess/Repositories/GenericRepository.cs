using MCT.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MCT.DataAccess.Repositories
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        internal EFContext context;
        internal DbSet<TEntity> dbSet;

        public GenericRepository(EFContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public GenericRepository()
        {
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public virtual TEntity Insert(TEntity entity)
        {
            try
            {
                dbSet.Add(entity);
                context.SaveChanges();
            }
            catch (Exception)
            {
                return null;
            }

            return entity;
        }

        public virtual Dictionary<TEntity, bool> InsertMany(List<TEntity> entities)
        {
            Dictionary<TEntity, bool> results = new Dictionary<TEntity, bool>();
            foreach (var entity in entities)
            {
                var result = Insert(entity);
                results.Add(entity, (result != null ? true : false));
            }
            return results;
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
            context.SaveChanges();
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
            context.SaveChanges();
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}