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
using System.Security.Claims;

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

        public async Task<ActionResult> Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<Producto>>("Cart") ?? new List<Producto>();

            // Cargar los métodos de pago
            var metodosPago = new List<SelectListItem>();
            using (SqlConnection connection = new SqlConnection(_cnx))
            {
                await connection.OpenAsync();

                using (SqlCommand cmm = new SqlCommand("SELECT ID_METODO_PAGO, METODO FROM METODO_PAGO", connection))
                {
                    using (SqlDataReader rd = await cmm.ExecuteReaderAsync())
                    {
                        while (await rd.ReadAsync())
                        {
                            metodosPago.Add(new SelectListItem
                            {
                                Value = rd["ID_METODO_PAGO"].ToString(),
                                Text = rd["METODO"].ToString()
                            });
                        }
                    }
                }
            }

            ViewBag.MetodosPago = metodosPago;  // Pasar los métodos de pago a la vista

            return View(cart);
        }


    
        [HttpPost]
        public async Task<ActionResult> ProcessPayment(int MetodoPago)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<Producto>>("Cart") ?? new List<Producto>();

            // Verificar si el carrito está vacío
            if (cart.Count == 0)
            {
                TempData["ErrorMessage"] = "El carrito está vacío. No se puede completar la compra.";
                return RedirectToAction("Cart");
            }

            // Obtener el ID del usuario autenticado
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "No se puede identificar al usuario. Por favor, inicie sesión.";
                return RedirectToAction("LoginCli", "LoginCli");
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_cnx))
                {
                    await connection.OpenAsync();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Registrar la venta (cabecera) utilizando el procedimiento almacenado
                            SqlCommand command = new SqlCommand("SP_REGISTRAR_VENTA", connection, transaction)
                            {
                                CommandType = CommandType.StoredProcedure
                            };

                            decimal total = cart.Sum(p => p.Precio); // Obtener el monto total de la compra

                            // Parámetros para el procedimiento almacenado
                            command.Parameters.AddWithValue("@ID_USUARIO", userId); // ID del usuario
                            command.Parameters.AddWithValue("@MONTO_TOTAL", total); // Monto total de la venta
                            command.Parameters.AddWithValue("@ID_METODO_PAGO", MetodoPago); // Método de pago

                            // Parámetro de salida para obtener el ID de la venta
                            SqlParameter idVentaParam = new SqlParameter("@ID_VENTA", SqlDbType.NVarChar)
                            {
                                Direction = ParameterDirection.Output,
                                Size = 8 // Asegurar que el tamaño sea de 8 caracteres
                            };
                            command.Parameters.Add(idVentaParam);

                            await command.ExecuteNonQueryAsync(); // Ejecutar el procedimiento almacenado

                            // Obtener el ID de la venta generado
                            string idVenta = (string)idVentaParam.Value;

                            // Registrar los detalles de la venta
                            // Registrar los detalles de la venta
                            foreach (var producto in cart)
                            {
                                SqlCommand detailCommand = new SqlCommand("SP_REGISTRAR_DETALLE_VENTA", connection, transaction)
                                {
                                    CommandType = CommandType.StoredProcedure
                                };
                                detailCommand.Parameters.AddWithValue("@ID_VENTA", idVenta); // ID de la venta registrada
                                detailCommand.Parameters.AddWithValue("@ID_PRODUCTO", producto.IdProducto); // ID del producto
                                detailCommand.Parameters.AddWithValue("@CANTIDAD", 1); // Asumir cantidad 1 (puedes ajustar según sea necesario)
                                detailCommand.Parameters.AddWithValue("@PRECIO_UNITARIO", producto.Precio); // Precio unitario

                                await detailCommand.ExecuteNonQueryAsync(); // Ejecutar el detalle de la venta
                            }


                            // Confirmar la transacción si todo salió bien
                            transaction.Commit();

                            // Limpiar el carrito de la sesión
                            HttpContext.Session.Remove("Cart");

                            TempData["SuccessMessage"] = "Compra completada con éxito.";
                            return RedirectToAction("ListarProductos"); // Redirigir a la página de productos
                        }
                        catch (Exception ex)
                        {
                            // Si ocurre algún error, hacer rollback y mostrar mensaje de error
                            transaction.Rollback();
                            TempData["ErrorMessage"] = "Ocurrió un error al procesar la compra: " + ex.Message;
                            return RedirectToAction("Cart");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Si ocurre algún error al conectar con la base de datos, mostrar mensaje de error
                TempData["ErrorMessage"] = "Error al conectar con la base de datos: " + ex.Message;
                return RedirectToAction("Cart");
            }
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
