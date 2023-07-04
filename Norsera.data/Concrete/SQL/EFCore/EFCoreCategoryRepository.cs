using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Norsera.data.Abstract;
using Norsera.entity;

namespace Norsera.data.Concrete.SQL.EFCore
{
    public class EFCoreCategoryRepository : EfCoreGenericRepository<Category, NorseraContext>, ICategoryRepository
    {
        public void DeleteFromCategory(int ProductId, int CategoryId)
        {
            using (var context = new NorseraContext())
            {
                var cmd ="delete from productcategory where ProductId=@p0 and CategoryId=@p1";
                context.Database.ExecuteSqlRaw(cmd,ProductId,CategoryId);
            }
        }

        public Category GetByIdWithProducts(int categoryId)
        {
            using(var context = new NorseraContext())
            {
                return context.Categories
                                .Where(i=>i.CategoryId==categoryId)
                                .Include(i=>i.ProductCategories)
                                .ThenInclude(i=>i.Product)
                                .FirstOrDefault();
            }
        }

        
    }
}