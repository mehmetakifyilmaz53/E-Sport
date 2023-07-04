using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Norsera.entity;

namespace Norsera.data.Abstract
{
    public interface ICategoryRepository : IRepository<Category>
    {
      

        Category GetByIdWithProducts(int categoryId);

        void DeleteFromCategory(int ProductId, int CategoryId);
        
        
    }
}