using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace ShoppingCart.Helpers
{

    public class BasketHelper
    {
        /// <summary>
        /// shoppingCart & checkOutCart files are stored in ~/App_Data/
        /// </summary>
        public static string shoppingCartFile = HttpContext.Current.Server.MapPath("~/App_Data/ShoppingCart.xml");
        public static string checkOutFile = HttpContext.Current.Server.MapPath("~/App_Data/checkOut.xml");

        /// <summary>
        /// This function will create an XML file and store it in the shoppingCartFile location. It will create a new file if it doesn't already exist.
        /// </summary>
        /// <param name="item">The request is consumed as a BasketItem object</param>
        public static void AddToCart(BasketItem item)
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;

            try
            {
                if (!File.Exists(shoppingCartFile))
                {
                    // Create the file.
                    using (XmlWriter xw = XmlWriter.Create(shoppingCartFile, xws))
                    {
                        XDocument xDoc = new XDocument(
                            new XDeclaration("1.0", "UTF-8", null),
                            new XElement("Items",
                                new XElement("Item",
                                    new XElement("Id", item.Id),
                                    new XElement("CheckOut", item.CheckOut),
                                    new XElement("Product",
                                        new XElement("Id", item.Prod.Id),
                                        new XElement("Name", item.Prod.Name),
                                        new XElement("Cost", item.Prod.Cost),
                                        new XElement("Image", item.Prod.Image)))));
                        xDoc.Save(xw);
                    }
                }
                else
                {
                    // Append to file
                    XDocument xDoc = XDocument.Load(shoppingCartFile);
                    var newElement = new XElement("Item",
                                        new XElement("Id", item.Id),
                                        new XElement("CheckOut", item.CheckOut),
                                        new XElement("Product",
                                            new XElement("Id", item.Prod.Id),
                                            new XElement("Name", item.Prod.Name),
                                            new XElement("Cost", item.Prod.Cost),
                                            new XElement("Image", item.Prod.Image)));
                    xDoc.Element("Items").Add(newElement);
                    xDoc.Save(shoppingCartFile);
                }
            }
            catch (Exception e) {
                Debug.Print(e.Message);
                Debug.Print(e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// This function is a "soft delete". By default, when a BasketItem is created; CheckOut is set to true; when removing an 
        /// item from the shopping cart, it will mark the CheckOut to false.
        /// </summary>
        /// <param name="id">id of BaseketItem.Id</param>
        public static void UpdateCheckOut(int id)
        {
            try
            {
                XDocument xDoc = XDocument.Load(shoppingCartFile);
                var checkOut = from co in xDoc.Elements("Items").Elements("Item")
                               where int.Parse(co.Element("Id").Value) == id
                               select co;

                checkOut.FirstOrDefault().SetElementValue("CheckOut", false);
                xDoc.Save(shoppingCartFile);
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                Debug.Print(e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Returns the shopping cart XML.
        /// </summary>
        /// <returns>Returns a list of BasketItems</returns>
        public static List<BasketItem> linqShoppingCart()
        {
            List<BasketItem> shoppingCart = null;
            try
            {
                shoppingCart = (from sp in XDocument.Load(shoppingCartFile).Descendants("Items").Elements("Item")
                                where sp.Element("CheckOut").Value == "true"
                                select new BasketItem
                                    {
                                        Id = int.Parse(sp.Element("Id").Value),
                                        Prod = new Product()
                                        {
                                            Id = int.Parse(sp.Element("Product").Element("Id").Value),
                                            Name = sp.Element("Product").Element("Name").Value,
                                            Cost = double.Parse(sp.Element("Product").Element("Cost").Value),
                                            Image = sp.Element("Product").Element("Image").Value
                                        }

                                    }).ToList();
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                Debug.Print(e.StackTrace);
                throw;
            }

            return shoppingCart;
        }

        /// <summary>
        /// This function will calculate the totalCost, the constructor in BasketTotal 
        /// will calculate the Shipping and then the grand total in one go.
        /// </summary>
        /// <returns>Returns a BasketTotal object</returns>
        public static double getShoppingCost()
        {
            double totalCost = 0.0;
            double shippingCost = 0.0;

            try
            {
                var CostsValues = (from cost in XDocument.Load(shoppingCartFile).Descendants("Items").Elements("Item")
                                   where cost.Element("CheckOut").Value == "true"
                                   select cost).ToList();

                foreach (XElement cost in CostsValues)
                {
                    totalCost += double.Parse(cost.Element("Product").Element("Cost").Value);
                }

                if (totalCost > 0 && totalCost <= 50.0)
                    shippingCost = 10.0;
                else if (totalCost > 50.0)
                    shippingCost = 20.0;
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                Debug.Print(e.StackTrace);
                throw;
            }

            return shippingCost;
        }

        /// <summary>
        /// This function will create a checkOut xml file for the finalising the cart
        /// </summary>
        public static void checkOut()
        {
            int counter = 0;
            List<CheckOut> cart = null;
            try
            {
                cart = (from checkOut in XDocument.Load(shoppingCartFile).Descendants("Items").Elements("Item")
                            where checkOut.Element("CheckOut").Value == "true"
                            group checkOut by checkOut.Element("Product").Element("Id").Value into checkOutFinal
                            select new CheckOut
                            {
                                Id = ++counter,
                                ProductId = int.Parse(checkOutFinal.Elements("Product").Elements("Id").FirstOrDefault().Value),
                                Name = checkOutFinal.Elements("Product").Elements("Name").FirstOrDefault().Value,
                                Qty = checkOutFinal.Elements("Product").Elements("Id").Count(),
                                Cost = double.Parse(checkOutFinal.Elements("Product").Elements("Cost").FirstOrDefault().Value)
                            }
                ).ToList();

                if (cart == null || cart.Count == 0)
                    throw new InvalidOperationException("Shopping cart is empty!!");

                // If the checkOut file exists delete it
                if (File.Exists(checkOutFile))
                    File.Delete(checkOutFile);

                // Create XML for checkOut with Shipping Total
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.Indent = true;

                using (XmlWriter xw = XmlWriter.Create(checkOutFile, xws))
                {
                    XDocument xNewDoc = new XDocument(
                            new XDeclaration("1.0", "UTF-8", null),
                            new XElement("CheckOutList",
                                new XElement("ShippingCost", getShoppingCost()))
                            );
                    xNewDoc.Save(xw);
                }

                // Add the products
                var xEle = from c in cart
                select new XElement("Item",
                    new XElement("Id", c.Id),
                    new XElement("ProductId", c.ProductId),
                    new XElement("Name", c.Name),
                    new XElement("Qty", c.Qty),
                    new XElement("Cost", c.Cost));

                XDocument xDoc = XDocument.Load(checkOutFile);
                xDoc.Element("CheckOutList").Add(xEle);
                xDoc.Save(checkOutFile);

            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                Debug.Print(e.StackTrace);
                throw;
            } 
        }
        

    }
}