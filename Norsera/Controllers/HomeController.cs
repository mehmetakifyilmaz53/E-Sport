using Microsoft.AspNetCore.Mvc;
using Norsera.business.Abstract;
using Norsera.Models;
using Norsera.data;

namespace Norsera.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult index()
        {
  

            return View();
        }
    }
}