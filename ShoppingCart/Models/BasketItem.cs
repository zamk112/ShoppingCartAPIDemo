using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ShoppingCart.Models
{
    public class BasketItem
    {
        public int Id { get; set; }
        public bool CheckOut { get; set; } = true;
        public Product Prod { get; set; }

    }
}