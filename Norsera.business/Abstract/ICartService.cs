using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Norsera.entity;

namespace Norsera.business.Abstract
{
    public interface ICartService
    {
        void InitializeCart(string userId);

        Cart GetCartByUserId(string userId);

        void AddToCart(string userId, int ProductId,int quantity);

        void DeleteFromCart(string userId, int productId);
    }
}