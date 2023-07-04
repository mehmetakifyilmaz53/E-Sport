using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Norsera.business.Abstract;
using Norsera.data.Abstract;
using Norsera.entity;

namespace Norsera.business.Concrete
{
    public class ProductManager : IProductService
    {
        private IProductRepository _productRepository;

        public ProductManager(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }



        public bool Create(Product entity)
        {
            if (Validation(entity))
            {
                _productRepository.Create(entity);
                return true;
            }

            return false;

        }

        public void Delete(Product entity)
        {
            _productRepository.Delete(entity);
        }

        public List<Product> Getall()
        {
            return _productRepository.Getall();
        }

        public Product GetById(int id)
        {
            return _productRepository.GetById(id);
        }

        public Product GetByIdWithCategories(int id)
        {
            return _productRepository.GetByIdWithCategories(id);

        }

        public int GetCountByCategory(string category)
        {
            return _productRepository.GetCountByCategory(category);
        }

        public List<Product> GetHomePageProducts()
        {
            return _productRepository.GetHomePageProducts();
        }

        public Product GetProductDetails(string url)
        {
            return _productRepository.GetDetails(url);
        }

        public List<Product> GetProductsByCategory(string name, int page, int pageSize)
        {
            return _productRepository.GetProductsByCategory(name, page, pageSize);
        }

        public List<Product> GetSearchResult(string searchString)
        {
            return _productRepository.GetSearchResult(searchString);
        }

        public void Update(Product entity)
        {

            _productRepository.Update(entity);
        }

        public bool Update(Product entity, int[] categoryIds)
        {
            if (Validation(entity))
            {
                if (categoryIds.Length == 0)
                {
                    ErrorMessage += "Ürün İçin en az bir category seçmelisiniz";
                    return false;
                }
                _productRepository.Update(entity, categoryIds);
                return true;
            }
            return false;

        }

        public string ErrorMessage { get; set; }
        public bool Validation(Product entity)
        {
            var isValid = true;
            if (string.IsNullOrEmpty(entity.Name))
            {
                ErrorMessage += "Ürün İsmi Girmelisiniz.\n";
                isValid = false;
            }

            if (entity.Price < 0)
            {
                ErrorMessage += "Ürün Fiyatı Negarif Olamaz.\n";
                isValid = false;
            }

            if (string.IsNullOrEmpty(entity.Url))
            {
                ErrorMessage += "Ürün Url'si Girmelisin.\n";
                isValid = false;
            }

            if (string.IsNullOrEmpty(entity.Describiton))
            {
                ErrorMessage += "Ürün Describtion'si Girmelisin.\n";
                isValid = false;
            }

             if (string.IsNullOrEmpty(entity.ImageUrl))
            {
                ErrorMessage += "Ürün fotoğrafı Girmelisin.\n";
                isValid = false;
            }

            return isValid;

        }
    }
}