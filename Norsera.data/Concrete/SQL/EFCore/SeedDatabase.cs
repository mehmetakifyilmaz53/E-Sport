using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Norsera.entity;

namespace Norsera.data.Concrete.SQL.EFCore
{
    public class SeedDatabase
    {
        public static void Seed()
        {
            var context = new NorseraContext();

            if (context.Database.GetPendingMigrations().Count()==0)
            {
                if (context.Products.Count() == 0)
                {
                    context.Products.AddRange(Products);
                    context.SaveChanges();
                }
                
            }
            context.SaveChanges();
        }

        private static Product[] Products =
        {
            new Product {Name="Norsera Hoodie", Price=5000,ImageUrl="https://pbs.twimg.com/profile_images/1651139432232153088/tsWVUQHV_400x400.png", Describiton="Hoodie"},
            new Product {Name="Norsera Hoodie", Price=4000,ImageUrl="https://skcfiles.mncdn.com/livephotos/8/S222530-001/332-02.jpg", Describiton="Hoodie"},
            new Product {Name="Norsera Hoodie", Price=8000,ImageUrl="https://w7.pngwing.com/pngs/175/902/png-transparent-black-hoodie-black-sweater-hoodie-thumbnail.png", Describiton="Hoodie"},
            new Product {Name="Norsera Hoodie", Price=3500,ImageUrl="https://e7.pngegg.com/pngimages/354/837/png-clipart-hoodie-t-shirt-clothing-bluza-zipper-hoodie-white-grey.png", Describiton="Hoodie"},
        };
    }
}