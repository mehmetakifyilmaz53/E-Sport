using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Norsera.entity;

namespace Norsera.data.Abstract
{
    public interface ICartRepository : IRepository<Cart>
    {
        Cart GetByUserId(string userId);

        void DeleteFromCart(int CartId, int productId);
    }
}