using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Norsera.data.Abstract;
using Norsera.entity;

namespace Norsera.data.Concrete.SQL.EFCore
{
    public class EFCoreProductRepository : EfCoreGenericRepository<Product, NorseraContext>, IProductRepository
    {
        public Product GetByIdWithCategories(int id)
        {
            using (var context = new NorseraContext())
            {
                return context.Products
                                .Where(i=>i.ProductId==id)
                                .Include(i=>i.ProductCategories)
                                .ThenInclude(i=>i.Category)
                                .FirstOrDefault();
            }
        }

        public int GetCountByCategory(string category)
        {
            using (var context = new NorseraContext())
            {
                var products = context.Products.AsQueryable();

                if (!string.IsNullOrEmpty(category))
                {
                    products = products
                                    .Include(i => i.ProductCategories)
                                    .ThenInclude(i => i.Category)
                                    .Where(i => i.ProductCategories.Any(a => a.Category.Url == category));

                }
                return products.Count();

            }
        }

        public Product GetDetails(string url)
        {
            using (var context = new NorseraContext())
            {
                return context.Products.Where(i => i.Url == url)
                                       .Include(i => i.ProductCategories)
                                       .ThenInclude(i => i.Category)
                                       .FirstOrDefault();
            }
        }

        public List<Product> GetHomePageProducts()
        {
            using (var context = new NorseraContext())
            {
                return context.Products.Where(i => i.IsHome).ToList();
            }
        }

        public List<Product> GetPopularProducts()
        {
            using (var context = new NorseraContext())
            {
                return context.Products.ToList();
            }
        }

        public List<Product> GetProductsByCategory(string name, int page, int pageSize)
        {
            using (var context = new NorseraContext())
            {
                var products = context.Products.AsQueryable();

                if (!string.IsNullOrEmpty(name))
                {
                    products = products
                                    .Include(i => i.ProductCategories)
                                    .ThenInclude(i => i.Category)
                                    .Where(i => i.ProductCategories.Any(a => a.Category.Url == name));

                }
                return products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                // return products.Skip(5).Take(5).ToList(); ilk 5 ürünü öteleyip 
                // page-1*pageSize girme nedenimiz eğer 0 girerse müşteri bütün ürünleri getirmek için
            }
        }

        public List<Product> GetSearchResult(string searchString)
        
        {
            
            using (var context = new NorseraContext())
            {
                var products = context.Products
                    .Where(i => i.IsApproved && (i.Name.ToLower().Contains(searchString.ToLower()) ||
                                                 i.Describiton.ToLower().Contains(searchString.ToLower())))
                    .AsQueryable();

                return products.ToList();
            }
        }

        public void Update(Product entity, int[] categoryIds)
        {
            using (var context = new NorseraContext())
            {
               var products = context.Products
                                        .Include(i=>i.ProductCategories)
                                        .FirstOrDefault(i=>i.ProductId==entity.ProductId);

                if (products!=null)
                {
                    products.Name = entity.Name;
                    products.Price= entity.Price;
                    products.Describiton = entity.Describiton;
                    products.Url = entity.Url;
                    products.ImageUrl = entity.ImageUrl;
                    
                    products.IsApproved = entity.IsApproved;
                    products.IsHome = entity.IsHome;

                    products.ProductCategories = categoryIds.Select(catid=>new ProductCategory()
                    {
                        ProductId=entity.ProductId,
                        CategoryId = catid
                    }).ToList();

                    context.SaveChanges();
                }
            }
        }
    }
}