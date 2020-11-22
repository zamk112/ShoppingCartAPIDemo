using ShoppingCart.Helpers;
using ShoppingCart.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ShoppingCart.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ProductsController : ApiController
    {
        List<Product> products = null;

        public ProductsController()
        {

            products = ProductsHelper.linqProducts();
            // I'm deleting the shopping cart file everytime the index page is loaded.
            if (File.Exists(BasketHelper.shoppingCartFile))
                File.Delete(BasketHelper.shoppingCartFile);
        }

        // GET: api/Products
        public HttpResponseMessage Get()
        {   
            if (products == null)
            {
                var exResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Cannot find Products"),
                    ReasonPhrase = "Cannot find Products"
                };
                throw new HttpResponseException(exResponse);
            }

            return Request.CreateResponse(HttpStatusCode.OK, products);
        }

        // GET: api/Products/5
        public HttpResponseMessage Get(int id)
        {

            if (products == null)
            {
                var exResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Cannot find Products"),
                    ReasonPhrase = "Cannot find Products"
                };
                throw new HttpResponseException(exResponse);
            }

            return Request.CreateResponse(HttpStatusCode.OK, products.Where(x => x.Id == id).FirstOrDefault());
        }
    }
}
