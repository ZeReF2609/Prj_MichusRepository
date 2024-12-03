using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Michus.Models;

namespace Michus.DAO
{
    public class ProductoDao
    {
        private readonly string _connectionString;

        public ProductoDao(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Producto>> ObtenerProductos()
        {
            var productos = new List<Producto>();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("sp_ObtenerProductos", connection) { CommandType = CommandType.StoredProcedure })
            {
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        productos.Add(new Producto
                        {
                            IdProducto = reader["ID_PRODUCTO"].ToString(),
                            ProdNom = reader["PROD_NOM"].ToString(),
                            ProdNomweb = reader["PROD_NOMWEB"].ToString(),
                            Descripcion = reader["DESCRIPCION"] as string,
                            IdCategoria = reader["ID_CATEGORIA"].ToString(),
                            ProdFchcmrl = reader["PROD_FCHCMRL"] != DBNull.Value
                                ? DateOnly.FromDateTime((DateTime)reader["PROD_FCHCMRL"])
                                : (DateOnly?)null,
                            Precio = (decimal)reader["PRECIO"],
                            Estado = (int)reader["ESTADO"],
                            Imagen = reader["IMAGEN"] as string  // Aquí se agrega la propiedad Imagen
                        });
                    }
                }
            }
            return productos;
        }


        public async Task<Producto> ObtenerProductoPorId(string idProducto)
        {
            Producto producto = null;
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("sp_ObtenerProductoPorId", connection) { CommandType = CommandType.StoredProcedure })
            {
                command.Parameters.AddWithValue("@IdProducto", idProducto);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        producto = new Producto
                        {
                            IdProducto = reader["ID_PRODUCTO"].ToString(),
                            ProdNom = reader["PROD_NOM"].ToString(),
                            ProdNomweb = reader["PROD_NOMWEB"].ToString(),
                            Descripcion = reader["DESCRIPCION"] as string,
                            IdCategoria = reader["ID_CATEGORIA"].ToString(),
                            ProdFchcmrl = reader["PROD_FCHCMRL"] != DBNull.Value
                                ? DateOnly.FromDateTime((DateTime)reader["PROD_FCHCMRL"])
                                : (DateOnly?)null,
                            Precio = (decimal)reader["PRECIO"],
                            Estado = (int)reader["ESTADO"],
                            Imagen = reader["IMAGEN"] as string  // Aquí se agrega la propiedad Imagen
                        };
                    }
                }
            }
            return producto;
        }

        //se modifico el insertarProducto
        public async Task<int> InsertarProductos(Producto producto, IFormFile imagenFile)
        {
            string idProducto = GenerarIdProducto();
            producto.IdProducto = idProducto;

            // Obtener la ruta física del directorio wwwroot
            string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            // Crear el directorio de imágenes si no existe
            string imagenDirectorio = Path.Combine(wwwrootPath, "assets", "imagen");
            Directory.CreateDirectory(imagenDirectorio);

            // Ruta física completa para guardar la imagen
            string rutaImagen = Path.Combine(imagenDirectorio, $"{producto.IdProducto}.jpg");

            // Ruta relativa para guardar en la base de datos
            string rutaImagenRelativa = Path.Combine("assets", "imagen", $"{producto.IdProducto}.jpg");

            if (imagenFile != null && imagenFile.Length > 0)
            {
                // Guardar la imagen en el sistema de archivos
                using (var stream = new FileStream(rutaImagen, FileMode.Create))
                {
                    await imagenFile.CopyToAsync(stream);
                }

                // Actualizar la ruta de la imagen en el modelo
                producto.Imagen = $"/{rutaImagenRelativa}"; // Añadir barra al inicio de la ruta
            }

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("sp_InsertarProducto", connection) { CommandType = CommandType.StoredProcedure })
            {
                command.Parameters.AddWithValue("@IdProducto", producto.IdProducto);
                command.Parameters.AddWithValue("@ProdNom", producto.ProdNom);
                command.Parameters.AddWithValue("@ProdNomWeb", producto.ProdNomweb);
                command.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                command.Parameters.AddWithValue("@IdCategoria", producto.IdCategoria);
                command.Parameters.AddWithValue("@ProdFchCmrl", producto.ProdFchcmrl.HasValue ? producto.ProdFchcmrl.Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value);
                command.Parameters.AddWithValue("@Precio", producto.Precio);
                command.Parameters.AddWithValue("@Estado", producto.Estado);
                command.Parameters.AddWithValue("@Imagen", producto.Imagen); // Nueva columna

                await connection.OpenAsync();
                var result = await command.ExecuteScalarAsync();

                return Convert.ToInt32(result);
            }
        }

        //fin de la modificacion
        private string GenerarIdProducto()
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = new SqlCommand("SELECT TOP 1 ID_PRODUCTO FROM PRODUCTO ORDER BY ID_PRODUCTO DESC", connection);
            var ultimoId = command.ExecuteScalar() as string;

            if (string.IsNullOrEmpty(ultimoId))
            {
                return "P001";
            }

            var numeroStr = ultimoId.Substring(1);
            if (int.TryParse(numeroStr, out int numero))
            {
                return $"P{(numero + 1):D3}";
            }

            throw new Exception("Formato inválido en el ID del producto.");
        }

        public async Task ActualizarProducto(Producto producto, IFormFile imagenFile)
        {
            // Obtener la ruta física del directorio wwwroot
            string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            // Crear el directorio de imágenes si no existe
            string imagenDirectorio = Path.Combine(wwwrootPath, "assets", "imagen");
            Directory.CreateDirectory(imagenDirectorio);

            // Ruta física completa para guardar la imagen
            string rutaImagen = Path.Combine(imagenDirectorio, $"{producto.IdProducto}.jpg");

            // Ruta relativa para guardar en la base de datos
            string rutaImagenRelativa = Path.Combine("assets", "imagen", $"{producto.IdProducto}.jpg");

            // Verificar si existe la imagen anterior
            string rutaImagenAnterior = Path.Combine(imagenDirectorio, $"{producto.IdProducto}.jpg");
            if (File.Exists(rutaImagenAnterior))
            {
                // Eliminar la imagen anterior
                File.Delete(rutaImagenAnterior);
            }

            if (imagenFile != null && imagenFile.Length > 0)
            {
                // Guardar la imagen en el sistema de archivos
                using (var stream = new FileStream(rutaImagen, FileMode.Create))
                {
                    await imagenFile.CopyToAsync(stream);
                }

                // Actualizar la ruta de la imagen en el modelo
                producto.Imagen = $"/{rutaImagenRelativa}"; // Añadir barra al inicio de la ruta

                // Agregar parámetro de consulta único para invalidar la caché
                producto.Imagen = $"/{rutaImagenRelativa}?v={DateTime.Now.Ticks}";
            }

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("sp_ActualizarProducto", connection) { CommandType = CommandType.StoredProcedure })
            {
                command.Parameters.AddWithValue("@IdProducto", producto.IdProducto);
                command.Parameters.AddWithValue("@ProdNom", producto.ProdNom);
                command.Parameters.AddWithValue("@ProdNomWeb", producto.ProdNomweb);
                command.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                command.Parameters.AddWithValue("@IdCategoria", producto.IdCategoria);
                command.Parameters.AddWithValue("@ProdFchCmrl", producto.ProdFchcmrl.HasValue ? producto.ProdFchcmrl.Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value);
                command.Parameters.AddWithValue("@Precio", producto.Precio);
                command.Parameters.AddWithValue("@Estado", producto.Estado);
                command.Parameters.AddWithValue("@Imagen", producto.Imagen); // Usar la ruta con barra inicial

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }



        public async Task ActivarProducto(string id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_ActivarProductos", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdProducto", id);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task DesactivarProducto(string id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_DesactivarProducto", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdProducto", id);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}

