using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Norsera.entity;

namespace Norsera.Models
{
    public class ProductModel
    {
        public int ProductId{get;set;}

        // [Display(Name="Product Name")]
        // [Required(ErrorMessage ="Name Zorunlu Bir alan.")]
        // [StringLength(60,MinimumLength =5,ErrorMessage ="Ürün ismi 5-10 karakter aralığında olmalı")]
        public string Name {get;set;}

        // [Required(ErrorMessage ="Url Zorunlu Bir alan.")]
        public string Url {get;set;}

        // [Required(ErrorMessage ="Price Zorunlu Bir alan.")]
        // [Range(1,10000,ErrorMessage ="Price için 1-10000 arasında değer girmelisiniz")]
        public int Price {get;set;}

        // [Required(ErrorMessage ="ImageUrl Zorunlu Bir alan.")]
        public string ImageUrl {get;set;}

        // [Required(ErrorMessage ="Describiton Zorunlu Bir alan.")]
        // [StringLength(60,MinimumLength =5,ErrorMessage ="Ürün ismi 5-10 karakter aralığında olmalı")]
        public string Describiton {get;set;}

        public bool IsHome {get;set;}

        public bool IsApproved {get;set;}

        public List<Category>? SelectedCategories { get; set; }

        
    }
}