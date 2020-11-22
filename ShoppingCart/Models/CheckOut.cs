using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCart.Models
{
    public class CheckOut
    {
        public int Id { get; set; } = 0;
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int Qty { get; set; } = 0;
        public double Cost { get; set; }

    }
}