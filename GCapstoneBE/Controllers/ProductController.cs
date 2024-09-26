using GCapstoneBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace GCapstoneBE.Controllers
{
    [RoutePrefix("api/product")]
    public class ProductController : ApiController
    {
        BarEntities db = new BarEntities();
        Response response = new Response();

        [HttpPost, Route("addNewProduct")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage AddNewProduct([FromBody] Product product)
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                if (tokenClaim.Role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                product.status = "true";
                db.Products.Add(product);
                db.SaveChanges();
                response.message = "Prodotto aggiunto dcon successo";
                return Request.CreateResponse(HttpStatusCode.OK, response);

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet, Route("getAllProduct")]
        [CustomAuthenticationFilter]

        public HttpResponseMessage GetAllProduct()
        {
            try
            {
                var result = from Products in db.Products
                             join Category in db.Categories
                             on Products.categoryld equals Category.id
                             select new
                             {
                                 Products.id,
                                 Products.name,
                                 Products.description,
                                 Products.price,
                                 Products.status,
                                 catgoryId = Category.id,
                                 categoryName = Category.name,

                             };
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

        }

        [HttpGet, Route("getProductByCategory/{id}")]
        [CustomAuthenticationFilter]

        public HttpResponseMessage GetProductByCategory(int id)
        {
            try
            {
                var result = db.Products
                    .Where(x => x.categoryld == id && x.status == "true")
                    .Select(x => new { x.id, x.name }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, result);


            }
            catch (Exception ex)
            {

                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }




        [HttpGet, Route("getProductById/{id}")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetProductById(int id)
        {
            try
            {
                Product productObj = db.Products.Find(id);
                return Request.CreateResponse(HttpStatusCode.OK, productObj);
            }
            catch (Exception ex)
            {


                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpPost, Route("updateProduct")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage UpdateProduct([FromBody] Product product)
        {
            try
            {

                var token = Request.Headers.GetValues("authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                if (tokenClaim.Role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                Product productObj = db.Products.Find(product.id);
                if (productObj == null)
                {
                    response.message = "Prodotto non trovato";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                productObj.name = product.name;
                productObj.categoryld = product.categoryld;
                productObj.description = product.description;
                productObj.price = product.price;
                db.Entry(productObj).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                response.message = "Prodotto aggiornato con successo";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);

            }
        }


        [HttpPost, Route("deletedProduct/{id}")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage DeleteProduct(int id)
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                if (tokenClaim.Role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                Product productObj = db.Products.Find(id);
                if (productObj == null)
                {
                    response.message = "Prodotto non torvato";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                db.Products.Remove(productObj);
                db.SaveChanges();
                response.message = "Prodotto cancellato con succeso";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);

            }
        }

        [HttpPost, Route("updateProductStatus")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage UpdateProductStatus([FromBody] Product product)
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                if (tokenClaim.Role != "admin")
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                Product productObj = db.Products.Find(product.id);
                if (productObj == null)
                {
                    response.message = "Prodotto non trovato";
                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }

                productObj.status = product.status;
                db.Entry(productObj).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                response.message = "Stato del prodotto aggiornato";
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);

            }

        }
    }

}
