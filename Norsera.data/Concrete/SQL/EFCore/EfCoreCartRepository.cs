using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Norsera.data.Abstract;
using Norsera.entity;

namespace Norsera.data.Concrete.SQL.EFCore
{
    public class EfCoreCartRepository : EfCoreGenericRepository<Cart, NorseraContext>, ICartRepository
    {
        public void DeleteFromCart(int cartId, int productId)
        {
            using (var context= new NorseraContext())
            {
                var cmd = @"delete from CartItems where CartId=@p0 and ProductId=@p1";
                context.Database.ExecuteSqlRaw(cmd,cartId,productId);
            }
        }

        public Cart GetByUserId(string userId)
        {
            using (var context= new NorseraContext())
            {
                return context.Carts
                            .Include(i=>i.CartItems)
                            .ThenInclude(i=>i.Product)
                            .FirstOrDefault(i=>i.UserId==userId);
                                
            }
        }

        public override void Update(Cart entity)
        {
            using (var context = new NorseraContext())
            {
                context.Carts.Update(entity);
                context.SaveChanges();
            }
            
        }


    }
}