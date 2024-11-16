using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Michus.Models;
using System.Diagnostics;
using System.Collections.Generic;

namespace Michus.DAO
{
    public class ProductoDAO
    {
        private readonly string _connectionString;

        public ProductoDAO(IConfiguration configuration)
        {
            // Cadena de conexión
            _connectionString = configuration.GetConnectionString("cn1")
                ?? throw new ArgumentNullException("La cadena de conexión 'cn1' no está configurada.");
        }

        // Insertar Producto
        public async Task<int> InsertarProducto(Producto producto)
        {
            int filasAfectadas;

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("sp_InsertarProducto", connection) { CommandType = CommandType.StoredProcedure })
            {
                command.Parameters.AddWithValue("@IdProducto", producto.IdProducto);
                command.Parameters.AddWithValue("@ProdNom", producto.ProdNom);
                command.Parameters.AddWithValue("@ProdNomWeb", producto.ProdNomweb);
                command.Parameters.AddWithValue("@Descripcion", (object)producto.Descripcion ?? DBNull.Value);
                command.Parameters.AddWithValue("@IdCategoria", producto.IdCategoria);
                command.Parameters.AddWithValue("@ProdFchCmrl", producto.ProdFchcmrl ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Precio", producto.Precio);
                command.Parameters.AddWithValue("@Estado", producto.Estado);

                await connection.OpenAsync();

                // Verificar si el producto ya existe
                using (var checkCommand = new SqlCommand("SELECT COUNT(1) FROM PRODUCTO WHERE ID_PRODUCTO = @IdProducto", connection))
                {
                    checkCommand.Parameters.AddWithValue("@IdProducto", producto.IdProducto);
                    var existe = (int)await checkCommand.ExecuteScalarAsync();

                    if (existe > 0)
                    {
                        // El producto ya existe, no lo insertamos
                        return 0;
                    }
                }

                // Si no existe, se inserta el nuevo producto
                filasAfectadas = await command.ExecuteNonQueryAsync();
            }
            return filasAfectadas;
        }

        // Obtener productos
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
                        DateTime? fechaComercialDateTime = reader["PROD_FCHCMRL"] as DateTime?;

                        DateOnly? fechaComercial = fechaComercialDateTime.HasValue
                            ? DateOnly.FromDateTime(fechaComercialDateTime.Value)
                            : (DateOnly?)null;

                        productos.Add(new Producto
                        {
                            IdProducto = reader["ID_PRODUCTO"].ToString(),
                            ProdNom = reader["PROD_NOM"].ToString(),
                            ProdNomweb = reader["PROD_NOMWEB"].ToString(),
                            Descripcion = reader["DESCRIPCION"] as string,
                            IdCategoria = reader["ID_CATEGORIA"].ToString(),
                            ProdFchcmrl = fechaComercial, // Asignar el valor DateOnly?
                            Precio = (decimal)reader["PRECIO"],
                            Estado = (int)reader["ESTADO"]
                        });
                    }
                }
            }
            return productos;
        }

        // Obtener producto por ID
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
                            ProdFchcmrl = reader["PROD_FCHCMRL"] as DateOnly?,
                            Precio = (decimal)reader["PRECIO"],
                            Estado = (int)reader["ESTADO"]
                        };
                    }
                }
            }
            return producto;
        }

        // Actualizar Producto
        public async Task<int> ActualizarProducto(Producto producto)
        {
            int filasAfectadas;
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("sp_ActualizarProducto", connection) { CommandType = CommandType.StoredProcedure })
            {
                command.Parameters.AddWithValue("@IdProducto", producto.IdProducto);
                command.Parameters.AddWithValue("@ProdNom", producto.ProdNom);
                command.Parameters.AddWithValue("@ProdNomWeb", producto.ProdNomweb);
                command.Parameters.AddWithValue("@Descripcion", (object)producto.Descripcion ?? DBNull.Value);
                command.Parameters.AddWithValue("@IdCategoria", producto.IdCategoria);
                command.Parameters.AddWithValue("@ProdFchCmrl", producto.ProdFchcmrl ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Precio", producto.Precio);
                command.Parameters.AddWithValue("@Estado", producto.Estado);

                await connection.OpenAsync();
                filasAfectadas = await command.ExecuteNonQueryAsync();
            }
            return filasAfectadas;
        }

        // Desactivar Producto
        public async Task<int> DesactivarProducto(string idProducto)
        {
            int filasAfectadas;
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("sp_DesactivarProducto", connection) { CommandType = CommandType.StoredProcedure })
            {
                command.Parameters.AddWithValue("@IdProducto", idProducto);

                await connection.OpenAsync();
                filasAfectadas = await command.ExecuteNonQueryAsync();
            }
            return filasAfectadas;
        }
    }
}
