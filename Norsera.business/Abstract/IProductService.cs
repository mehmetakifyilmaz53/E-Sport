using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Norsera.entity;

namespace Norsera.business.Abstract
{
    public interface IProductService : IValidator<Product>
    {
        List<Product> Getall();

        Product GetByIdWithCategories(int id);

        List<Product> GetProductsByCategory(string name, int page,int pageSize);

        Product GetProductDetails(string url);

        public Product GetById(int id);

        public bool Create(Product entity);

        public void Update (Product entity);

        public void Delete (Product entity);
        int GetCountByCategory(string category);

        List<Product> GetHomePageProducts();
        List<Product> GetSearchResult(string searchString);
        bool Update(Product entity, int[] categoryIds);
    }
}