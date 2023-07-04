using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Norsera.business.Abstract;
using Norsera.data;

namespace Norsera.ViewComponents
{
    public class CategoriesViewComponent : ViewComponent
    {
        private ICategoryService _categoryService;

        public CategoriesViewComponent (ICategoryService categoryService)
        {
            this._categoryService = categoryService;
        }

        public IViewComponentResult Invoke()
        {
            //burayı değiştirmeyi unutma
            ViewBag.SelectedCategory = RouteData.Values["category"];
            return View(_categoryService.Getall());
        }
    }
}