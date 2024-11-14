using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Michus.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Michus.util;

namespace Michus.Controllers
{
    public class EcommerceController : Controller
    {
        private readonly string _cnx;

        public EcommerceController(IConfiguration configuration)
        {
            _cnx = configuration.GetConnectionString("cn1")!;
        }

        // 1. LISTING PRODUCTS

        public async Task<ActionResult> ListarProductos(string? category)
        {
            var productos = new List<Producto>();

            using (SqlConnection connection = new SqlConnection(_cnx))
            {
                await connection.OpenAsync();

                // If a category is selected, use the procedure that accepts the category parameter
                SqlCommand cmm;
                if (!string.IsNullOrEmpty(category))
                {
                    cmm = new SqlCommand("SP_LISTAR_PRODUCTO_BY_CATEGORY", connection);
                    cmm.Parameters.Add(new SqlParameter("@ID_CATEGORIA", category));
                }
                else
                {
                    // Otherwise, use the procedure that doesn't take parameters
                    cmm = new SqlCommand("SP_LISTAR_PRODUCTO", connection);
                }

                cmm.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader rd = await cmm.ExecuteReaderAsync())
                {
                    while (await rd.ReadAsync())
                    {
                        productos.Add(new Producto
                        {
                            IdProducto = rd["ID_PRODUCTO"].ToString(),
                            ProdNom = rd["PROD_NOM"].ToString(),
                            ProdNomweb = rd["PROD_NOMWEB"].ToString(),
                            Descripcion = rd["DESCRIPCION"].ToString(),
                            IdCategoria = rd["ID_CATEGORIA"].ToString(),
                            Precio = (decimal)rd["PRECIO"],
                            Estado = (int)rd["ESTADO"]
                        });
                    }
                }
            }

            // Fetch categories for the filter dropdown
            var categories = new List<SelectListItem>();
            using (SqlConnection connection = new SqlConnection(_cnx))
            {
                await connection.OpenAsync();

                using (SqlCommand cmm = new SqlCommand("SP_LISTAR_CATEGORIAS", connection))
                {
                    cmm.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader rd = await cmm.ExecuteReaderAsync())
                    {
                        while (await rd.ReadAsync())
                        {
                            categories.Add(new SelectListItem
                            {
                                Value = rd["ID_CATEGORIA"].ToString(),
                                Text = rd["CATEGORIA"].ToString()
                            });
                        }
                    }
                }
            }

            // Ensure categories are not null
            ViewBag.Categories = categories ?? new List<SelectListItem>();

            return View(productos); // Pass the list of products as the model
        }




        // 2. LISTING PROMOTIONS







        // 3. SHOPPING CART

        public ActionResult Cart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<Producto>>("Cart") ?? new List<Producto>();
            return View(cart);
        }

        [HttpPost]
        public ActionResult AddToCart(string productId)
        {
            var product = GetProductById(productId);

            if (product != null)
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<Producto>>("Cart") ?? new List<Producto>();
                cart.Add(product);
                HttpContext.Session.SetObjectAsJson("Cart", cart);

                // Usamos TempData para pasar un mensaje de confirmación
                TempData["Message"] = $"{product.ProdNom} ha sido agregado al carrito.";
            }

            return RedirectToAction("ListarProductos");
        }



        [HttpPost]
        public ActionResult RemoveFromCart(string productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<Producto>>("Cart") ?? new List<Producto>();
            var item = cart.Find(x => x.IdProducto == productId);

            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return RedirectToAction("Cart");
        }

        // 4. CHECKOUT

        public ActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<Producto>>("Cart") ?? new List<Producto>();
            return View(cart);
        }

        [HttpPost]
        public ActionResult ProcessPayment()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<Producto>>("Cart") ?? new List<Producto>();

            // Payment processing logic can be added here

            HttpContext.Session.Remove("Cart"); // Clear the cart after payment
            return RedirectToAction("ListarProductos");
        }

        // HELPER METHOD TO GET A PRODUCT BY ID

        private Producto GetProductById(string productId)
        {
            using (SqlConnection connection = new SqlConnection(_cnx))
            {
                connection.Open();

                using (SqlCommand cmm = new SqlCommand("SP_GET_PRODUCTO_BY_ID", connection))
                {
                    cmm.CommandType = CommandType.StoredProcedure;
                    cmm.Parameters.Add(new SqlParameter("@ID_PRODUCTO", productId));

                    using (SqlDataReader rd = cmm.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            return new Producto
                            {
                                IdProducto = rd["ID_PRODUCTO"].ToString(),
                                ProdNom = rd["NOMBRE"].ToString(),
                                ProdNomweb = rd["NOMBRE_WEB"].ToString(),
                                Descripcion = rd["DESCRIPCION"].ToString(),
                                IdCategoria = rd["ID_CATEGORIA"].ToString(),
                                Precio = (decimal)rd["PRECIO"],
                                Estado = (int)rd["ESTADO"]
                            };
                        }
                    }
                }
            }
            return null;
        }

    }
}
