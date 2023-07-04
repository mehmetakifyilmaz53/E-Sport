using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Norsera.business.Abstract;
using Norsera.data.Abstract;
using Norsera.entity;

namespace Norsera.business.Concrete
{
    public class CategoryManager : ICategoryService
    {
        private ICategoryRepository _categoryRepository;

        public CategoryManager(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        

        public void Create(Category entity)
        {
            _categoryRepository.Create(entity);
        }

        public void Delete(Category entity)
        {
            _categoryRepository.Delete(entity);
        }

        public void DeleteFromCategory(int ProductId, int CategoryId)
        {
            _categoryRepository.DeleteFromCategory(ProductId,CategoryId);
        }

        public List<Category> Getall()
        {
            return _categoryRepository.Getall();
        }

        public Category GetById(int id)
        {
            return _categoryRepository.GetById(id);
        }

        public Category GetByIdWithProducts(int categoryId)
        {
            return _categoryRepository.GetByIdWithProducts(categoryId);
        }

        public void Update(Category entity)
        {
            _categoryRepository.Update(entity);
        }

        public string ErrorMessage { get; set; }
        public bool Validation(Category entity)
        {
            var isValid = true;

        
            return isValid;
        }
    }
}