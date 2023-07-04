using Norsera.entity;

namespace Norsera.Models
{
    public class ProductDetailModel
    {
        public Product? Product {get;set;}   

        public List<Category>? Categories {get;set;}
    }
}