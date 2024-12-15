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
    using Michus.DAO;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;

    namespace Michus.Controllers
    {
        public class EcommerceController : Controller
        {
        private readonly string _cnx;
        private readonly IWebHostEnvironment _env;

        // Constructor que recibe tanto la configuración como el IWebHostEnvironment
        public EcommerceController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _cnx = configuration.GetConnectionString("cn1")!;
            _env = env;
        }

        // 1. LISTING PRODUCTS

        public async Task<ActionResult> ListarProductos(string? category)
            {
                var productos = new List<Producto>();

                using (SqlConnection connection = new SqlConnection(_cnx))
                {
                    await connection.OpenAsync();

                    SqlCommand cmm;
                    if (!string.IsNullOrEmpty(category))
                    {
                        cmm = new SqlCommand("SP_LISTAR_PRODUCTO_BY_CATEGORY", connection);
                        cmm.Parameters.Add(new SqlParameter("@ID_CATEGORIA", category));
                    }
                    else
                    {
                        cmm = new SqlCommand("SP_LISTAR_PRODUCTO", connection);
                    }

                    cmm.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader rd = await cmm.ExecuteReaderAsync())
                    {
                        while (await rd.ReadAsync())
                        {
                            productos.Add(new Producto
                            {
                                IdProducto = rd["ID_PRODUCTO"].ToString()!,
                                ProdNom = rd["PROD_NOM"].ToString()!,
                                ProdNomweb = rd["PROD_NOMWEB"].ToString()!,
                                Descripcion = rd["DESCRIPCION"].ToString(),
                                Precio = (decimal)rd["PRECIO"],
                                Estado = (int)rd["ESTADO"],
                                // Verificar si la columna "IMAGEN" existe antes de asignar el valor
                                Imagen = rd.GetOrdinal("IMAGEN") >= 0 ? rd["IMAGEN"].ToString() : string.Empty
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

            public async Task<ActionResult> HistorialCompras()
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["ErrorMessage"] = "No se puede identificar al usuario. Por favor, inicie sesión.";
                    return RedirectToAction("LoginCli", "LoginCli");
                }

                var ventas = new List<Venta>();
                using (SqlConnection connection = new SqlConnection(_cnx))
                {
                    await connection.OpenAsync();
                    using (SqlCommand cmm = new SqlCommand("SP_LISTAR_VENTAS_POR_USUARIO", connection))
                    {
                        cmm.CommandType = CommandType.StoredProcedure;
                        cmm.Parameters.Add(new SqlParameter("@ID_USUARIO", userId));

                        using (SqlDataReader rd = await cmm.ExecuteReaderAsync())
                        {
                            Venta currentVenta = null;
                            while (await rd.ReadAsync())
                            {
                                // If this is a new sale, create a new Venta object
                                if (currentVenta == null || currentVenta.IdVenta != rd["ID_VENTA"].ToString())
                                {
                                    currentVenta = new Venta
                                    {
                                        IdVenta = rd["ID_VENTA"].ToString(),
                                        FechaVenta = (DateTime)rd["FECHA_VENTA"],
                                        MontoTotal = (decimal)rd["MONTO_TOTAL"],
                                        IdMetodoPago = Convert.ToInt32(rd["ID_METODO_PAGO"]),
                                        Estado = (int)rd["ESTADO"],
                                        IdMetodoPagoNavigation = new MetodoPago
                                        {
                                            Metodo = rd["METODO_PAGO"].ToString()
                                        },
                                        Detalles = new List<VentaDetalle>()
                                    };
                                    ventas.Add(currentVenta);
                                }

                                // Add product details if they exist
                                if (rd["ID_PRODUCTO"] != DBNull.Value)
                                {
                                    currentVenta.Detalles.Add(new VentaDetalle
                                    {
                                        IdProducto = rd["ID_PRODUCTO"].ToString(),
                                        NombreProducto = rd["NOMBRE_PRODUCTO"].ToString(),
                                        Cantidad = Convert.ToInt32(rd["CANTIDAD"]),
                                        PrecioUnitario = (decimal)rd["PRECIO_UNITARIO"]
                                    });
                                }
                            }
                        }
                    }
                }
                return View(ventas);
            }


        // 4. RESERVAS
        public async Task<ActionResult> Reservas()
        {
            var mesasDisponibles = new List<SelectListItem>();

            using (SqlConnection connection = new SqlConnection(_cnx))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("SP_LISTARMESAS", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (reader["DISPONIBILIDAD"].ToString() == "Disponible")
                            {
                                mesasDisponibles.Add(new SelectListItem
                                {
                                    Value = reader["ID_MESA"].ToString(),
                                    Text = "Mesa " + reader["NUMERO_MESA"] + " - " + reader["ASIENTOS"] + " asientos"
                                });
                            }
                        }
                    }
                }
            }
            ViewBag.Mesas = mesasDisponibles;

            return View();
        }

        public async Task<IActionResult> CrearReserva([FromBody] ReservaModel reserva)
        {
            var resultado = new { success = false, message = "Hubo un error al procesar la reserva." };

            // Validar fechaReserva
            DateTime fecha;
            if (!DateTime.TryParseExact(reserva.FechaReserva, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
            {
                resultado = new { success = false, message = "La fecha de reserva no es válida." };
                return Json(resultado);
            }

            // Validar horaReserva
            TimeSpan hora;
            if (!TimeSpan.TryParseExact(reserva.HoraReserva, "hh\\:mm", CultureInfo.InvariantCulture, out hora))
            {
                resultado = new { success = false, message = "La hora de reserva no es válida." };
                return Json(resultado);
            }

            using (SqlConnection connection = new SqlConnection(_cnx))
            {
                try
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SP_Reservas", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros de la consulta
                        command.Parameters.AddWithValue("@Accion", 1);
                        command.Parameters.AddWithValue("@Nombre_Usuario", reserva.CorreoElectronico);
                        command.Parameters.AddWithValue("@ID_Mesa", reserva.MesaID);
                        command.Parameters.AddWithValue("@Fecha_Reserva", fecha);
                        command.Parameters.AddWithValue("@Hora_Reserva", hora);
                        command.Parameters.AddWithValue("@Cantidad_Personas", reserva.CantidadPersonas);

                        // Ejecución del SP y captura de resultados
                        var result = await command.ExecuteReaderAsync();

                        // Leer el resultado
                        if (result.HasRows)
                        {
                            while (await result.ReadAsync())
                            {
                                var mensaje = result["Mensaje"].ToString();
                                if (mensaje == "Reserva creada exitosamente.")
                                {
                                    // Obtener el ID de la reserva (como string)
                                    var idReserva = result["ID_Reserva"].ToString();

                                    // Enviar correo de confirmación
                                    await EnviarCorreoConfirmacion(reserva.CorreoElectronico, idReserva, fecha, hora, _env);

                                    resultado = new { success = true, message = mensaje };
                                }
                                else
                                {
                                    resultado = new { success = false, message = mensaje };
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Si ocurre algún error en la conexión o en la ejecución
                    resultado = new { success = false, message = "Hubo un error al intentar realizar la reserva: " + ex.Message };
                }
            }

            // Devolver el resultado como JSON
            return Json(resultado);
        }



        public async Task EnviarCorreoConfirmacion(string correoDestino, string idReserva, DateTime fechaReserva, TimeSpan horaReserva, IWebHostEnvironment env)
        {
            try
            {
                var correoOrigen = "josejuliosanchezcruzado1@gmail.com";
                var contraseñaCorreo = "povg qncq tphk urnk";

                var asunto = "Confirmación de Reserva";

                string rutaWebRoot = env.WebRootPath;
                string rutaImagen = Path.Combine(rutaWebRoot, "assets", "img", "mishus.png");

                // Cuerpo del correo con diseño HTML
                var cuerpo = $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f9;
            color: #333;
            margin: 0;
            padding: 0;
        }}
        .container {{
            width: 100%;
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            text-align: center;
            padding-bottom: 20px;
        }}
        .header img {{
            max-width: 150px;
        }}
        .content {{
            text-align: left;
            margin: 20px 0;
        }}
        .footer {{
            text-align: center;
            font-size: 12px;
            color: #777;
            margin-top: 20px;
        }}
        .important {{
            font-weight: bold;
            color: #ff6f61;
        }}
        .details {{
            padding: 10px;
            background-color: #f0f8ff;
            border-radius: 5px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <img src='cid:MichiLogo' alt='Logo Michi Restaurante'>
        </div>
        <div class='content'>
            <h2>¡Gracias por su reserva!</h2>
            <p>Estimado cliente,</p>
            <p>Le confirmamos que hemos recibido su reserva con los siguientes detalles:</p>
            <div class='details'>
                <p><span class='important'>ID de reserva:</span> {idReserva}</p>
                <p><span class='important'>Fecha de reserva:</span> {fechaReserva:yyyy-MM-dd}</p>
                <p><span class='important'>Hora de reserva:</span> {horaReserva.ToString(@"hh\:mm")}</p> 
            </div>
            <p>Le recordamos que si no asiste en los próximos 20 minutos luego de la hora acordadá, la mesa será liberada.</p>
        </div>
        <div class='footer'>
            <p>Gracias por elegirnos.</p>
        </div>
    </div>
</body>
</html>";

                var smtpCliente = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(correoOrigen, contraseñaCorreo),
                    EnableSsl = true
                };

                var mensaje = new MailMessage(correoOrigen, correoDestino, asunto, cuerpo)
                {
                    IsBodyHtml = true  
                };

                var imagen = new Attachment(rutaImagen)
                {
                    ContentId = "MichiLogo",
                    ContentType = new System.Net.Mime.ContentType("image/png")
                };
                mensaje.Attachments.Add(imagen);

                // Enviar el correo
                await smtpCliente.SendMailAsync(mensaje);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al enviar correo: {ex.Message}");
            }
        }

    }
}
