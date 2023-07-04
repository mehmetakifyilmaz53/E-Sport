using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Norsera.data.Abstract;

namespace Norsera.data.Concrete.SQL.EFCore
{
    public class EfCoreGenericRepository<Tentity, TContext> : IRepository<Tentity>
    where Tentity:class
    where TContext: DbContext, new ()
    {
        public void Create(Tentity entity)
        {
            using (var context = new TContext())
            {
                context.Set<Tentity>().Add(entity);
                context.SaveChanges();
            }
        }

        public void Delete(Tentity entity)
        {
            using (var context = new TContext())
            {
                context.Set<Tentity>().Remove(entity);
                context.SaveChanges();
            }
        }

        public List<Tentity> Getall()
        {
            using (var context = new TContext())
            {
                return context.Set<Tentity>().ToList();
            }
        }

        public Tentity GetById(int id)
        {
            using (var context = new TContext())
            {
                return context.Set<Tentity>().Find(id);
            }
        }

        public virtual void Update(Tentity entity)
        {
            using (var context = new TContext())
            {
                context.Entry(entity).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}