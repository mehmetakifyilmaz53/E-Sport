using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Norsera.entity;

namespace Norsera.business.Abstract
{
    public interface ICategoryService : IValidator<Category>
    {
        List<Category> Getall();

        public Category GetById(int id);

        public void Create(Category entity);

        public void Update (Category entity);

        public void Delete (Category entity);
        Category GetByIdWithProducts(int categoryId);

        void DeleteFromCategory(int ProductId,int CategoryId);
    }
}