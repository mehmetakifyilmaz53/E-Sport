using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Norsera.entity;

namespace Norsera.Models
{
    public class CategoryModel
    {
        public int CategoryId {get;set;}
        [Required(ErrorMessage ="Category name is required")]
        [StringLength(20,MinimumLength =1,ErrorMessage ="Please enter a value between 1 and 20 characters for the category")]
        public string? Name {get;set;}
        
        [Required(ErrorMessage ="Description is required")]
        [StringLength(100,MinimumLength =1,ErrorMessage ="Please enter a value between 1 and 100 characters for the description")]
        public string? Describiton {get;set;}

        [Required(ErrorMessage ="URL is required")]
        [StringLength(100,MinimumLength =1,ErrorMessage ="Please enter a value between 1 and 100 characters for the URL")]
        public string? Url {get;set;}
        
        public List<Product>? Products {get;set;}
    }

}