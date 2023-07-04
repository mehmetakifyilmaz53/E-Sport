using Microsoft.AspNetCore.Mvc;
using Norsera.business.Abstract;
using Norsera.entity;
using Norsera.Models;


namespace Norsera.Controllers
{
    public class ShopController : Controller
    {

        private IProductService _productService;

        public ShopController(IProductService productService)
        {
            this._productService = productService;
        }
        public IActionResult index()
        {
            var ProducViewModel = new ProductList()
            {
                Products = _productService.Getall(),
                HomeProducts = _productService.GetHomePageProducts()
            };

            return View(ProducViewModel);
        }

        public IActionResult List(string Category, int page = 1)
        {
            const int pageSize = 4;
            var ProductViewModel = new ProductList()
            {
                PageInfo = new PageInfo()
                {
                    Totalitems = _productService.GetCountByCategory(Category),
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    CurrenctCategory = Category
                },
                Products = _productService.GetProductsByCategory(Category, page, pageSize)
            };
            return View(ProductViewModel);
        }


        public IActionResult Details(string url)
        {
            if (url == null)
            {
                return NotFound();
            }
            Product product = _productService.GetProductDetails(url);

            if (product == null)
            {
                return NotFound();
            }
            return View(new ProductDetailModel
            {
                Product = product,
                Categories = product.ProductCategories.Select(i => i.Category).ToList()
            });
        }

        public IActionResult Search(string q)
        {
            if (q != null)
            {
                var ProductViewModel = new ProductList()
                {

                    Products = _productService.GetSearchResult(q)
                };
                return View(ProductViewModel);
            }

            return Redirect("/shop/list");
        }
    }
}