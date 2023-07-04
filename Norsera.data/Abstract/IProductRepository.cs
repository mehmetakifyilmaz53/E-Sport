using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Norsera.entity;

namespace Norsera.data.Abstract
{
    public interface IProductRepository : IRepository<Product>
    {
        Product GetDetails(string url);
        List<Product> GetPopularProducts();
        List<Product> GetProductsByCategory (string name,int page,int pageSize);
        int GetCountByCategory(string category);

         List<Product> GetHomePageProducts();

         List<Product> GetSearchResult(string searchString);

        Product GetByIdWithCategories(int id);
        void Update(Product entity, int[] categoryIds);
    }
}