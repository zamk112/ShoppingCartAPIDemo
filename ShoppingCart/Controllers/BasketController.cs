using ShoppingCart.Helpers;
using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ShoppingCart.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BasketController : ApiController
    {
        // GET: api/Basket
        public HttpResponseMessage Get()
        {
           List<BasketItem> basketItems = null;
           try
            {
                basketItems  = BasketHelper.linqShoppingCart();
            } catch(Exception e)
            {
                var exResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Cannot get cart items"),
                    ReasonPhrase = e.Message
                };
                throw new HttpResponseException(exResponse);
            }

            return Request.CreateResponse(HttpStatusCode.OK, basketItems);
        }


        // POST: api/Basket
        public HttpResponseMessage Post([FromBody]BasketItem item)
        {
            try
            {
                BasketHelper.AddToCart(item);
            }
            catch (Exception e)
            {
                var exResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Cannot Add Item to cart"),
                    ReasonPhrase = e.Message
                };
                throw new HttpResponseException(exResponse);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // PUT: api/Basket/5
        public HttpResponseMessage Put(int id)
        {
            try
            {
                BasketHelper.UpdateCheckOut(id);
            }
            catch (Exception e)
            {
                var exResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Cannot Update Item in Cart"),
                    ReasonPhrase = e.Message
                };
                throw new HttpResponseException(exResponse);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("api/Basket/getShippingCost")]
        [HttpGet]
        public HttpResponseMessage getShippingCost()
        {
            double shippingTotal = 0.0;

            try
            {
                shippingTotal = BasketHelper.getShoppingCost();
            }
            catch(Exception e)
            {
                var exResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Cannot get basket total"),
                    ReasonPhrase = e.Message
                };
                throw new HttpResponseException(exResponse);
            }


            return Request.CreateResponse(HttpStatusCode.OK, shippingTotal);
        }

        [Route("api/Basket/checkOut")]
        [HttpPost]
        public HttpResponseMessage checkOut()
        {
            try
            {
                BasketHelper.checkOut();
            }
            catch (Exception e)
            {
                var exResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Cannot complete check out"),
                    ReasonPhrase = e.Message
                };
                throw new HttpResponseException(exResponse);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
