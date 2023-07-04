using Norsera.entity;

namespace Norsera.Models
{
    public class PageInfo
    {
        public int Totalitems {get;set;}
        public int  ItemsPerPage {get;set;}
        public int CurrentPage {get;set;}

        public string? CurrenctCategory {get;set;}

        public int TotalPages()
        {
            return (int)Math.Ceiling(((decimal)Totalitems/ItemsPerPage));
        }
    }


    public class ProductList
    {
        public PageInfo? PageInfo { get; set; }
        public List<Product>? Products { get; set; }

        public List<Product>? HomeProducts {get;set;}
    }

}