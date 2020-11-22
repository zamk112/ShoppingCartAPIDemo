using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace ShoppingCart.Models
{
    public class ProductsHelper
    {
        /// <summary>
        /// This function returns a list of products
        /// </summary>
        public static List<Product> linqProducts()
        {
            List<Product> products = null;

            try
            {
                products = (
                  from p in XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/Products.xml")).Element("Products").Elements("Product")
                  select new Product
                  {
                      Id = int.Parse(p.Element("Id").Value),
                      Name = p.Element("Name").Value,
                      Cost = double.Parse(p.Element("Cost").Value),
                      Image = p.Element("Image").Value
                  }
              ).ToList();
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                Debug.Print(e.StackTrace);
                throw;
            }

            return products;
        }
    }
}