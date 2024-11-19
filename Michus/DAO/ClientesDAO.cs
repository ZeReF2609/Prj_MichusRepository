using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Michus.Models;
using Michus.Models_Store;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Michus.DAO
{
    public class ClientesDAO
    {
        private readonly string _connectionString;

        public ClientesDAO(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public List<ClienteCompleto> ObtenerTodosClientes(string idCliente = null)
        {
            List<ClienteCompleto> clientes = new List<ClienteCompleto>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("sp_CRUD_Clientes", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Parámetro de acción
                command.Parameters.AddWithValue("@Accion", 2); // Acción 2 para obtener clientes

                // Parámetro opcional ID_CLIENTE
                if (!string.IsNullOrEmpty(idCliente))
                {
                    command.Parameters.AddWithValue("@ID_CLIENTE", idCliente);
                }
                else
                {
                    command.Parameters.AddWithValue("@ID_CLIENTE", DBNull.Value);
                }

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Cliente cliente = new Cliente
                        {
                            IdCliente = reader["CODIGO"].ToString(),
                            Nombres = reader["NOMBRES"].ToString(),
                            Apellidos = reader["APELLIDOS"].ToString(),
                            DocIdent = reader["DNI"].ToString(),
                            FechaNacimiento = reader["CUMPLEANOS"] != DBNull.Value
                                ? DateOnly.FromDateTime(Convert.ToDateTime(reader["CUMPLEANOS"]))
                                : DateOnly.MinValue,
                            FechaRegistro = reader["INGRESO"] != DBNull.Value
                                ? Convert.ToDateTime(reader["INGRESO"])
                                : (DateTime?)null,
                            NivelFidelidad = reader["NIVEL"] != DBNull.Value
                                ? Convert.ToByte(reader["NIVEL"])
                                : (byte)0,
                            PuntosFidelidad = reader["PUNTOS"] != DBNull.Value
                                ? Convert.ToInt32(reader["PUNTOS"])
                                : 0
                        };

                        Contacto contacto = new Contacto
                        {
                            IdUsuario = reader["CODIGO"].ToString(),
                            Telefono = reader["TELEFONO"].ToString(),
                            Direccion = reader["DIRECCION"].ToString()
                        };

                        UsuariosSistema usuarioSistema = new UsuariosSistema
                        {
                            IdUsuario = reader["CODIGO"].ToString(),
                            Estado = reader["ESTADO"] != DBNull.Value
                                ? Convert.ToByte(reader["ESTADO"])
                                : (byte)0,
                            IdRol = reader["ID_ROL"].ToString()
                        };

                        // Crear una instancia de ClienteCompleto y agregarla a la lista
                        clientes.Add(new ClienteCompleto
                        {
                            Cliente = cliente,
                            Contacto = contacto,
                            Sistema = usuarioSistema
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return clientes;
        }


        public List<TipoDocumento> ObtenerTiposDocumento()
        {
            var tipoDocumentos = new List<TipoDocumento>();

            using var connection = new SqlConnection(_connectionString);
            connection.Open(); // Abrir la conexión de forma síncrona

            using var command = new SqlCommand("SP_LISTAR_TIPO_DOCUMENTO", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            using var reader = command.ExecuteReader();
            while (reader.Read()) 
            {
                var tipoDocumento = new TipoDocumento
                {
                    IdDoc = reader.GetInt32(reader.GetOrdinal("ID_DOC")),
                    Descripcion = reader.IsDBNull(reader.GetOrdinal("DESCRIPCION")) ? string.Empty : reader.GetString(reader.GetOrdinal("DESCRIPCION"))
                };

                tipoDocumentos.Add(tipoDocumento);
            }

            return tipoDocumentos;
        }


        public bool InsertarCliente(string usuario, string email, string contrasenia, string nombres, string apellidos, int idDoc, string docIdent, DateTime fechaNacimiento, int nivelFidelidad, int puntosFidelidad, string telefono, string direccion, int accion)
        {
            string idCliente = GenerarIdCliente();

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = new SqlCommand("sp_CRUD_Clientes", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Parámetros de entrada
            command.Parameters.AddWithValue("@ACCION", 1);
            command.Parameters.AddWithValue("@ID_CLIENTE", idCliente);
            command.Parameters.AddWithValue("@NOMBRES", nombres ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@APELLIDOS", apellidos ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ID_DOC", idDoc);
            command.Parameters.AddWithValue("@DOC_IDENT", docIdent ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@FECHA_NACIMIENTO", fechaNacimiento);
            command.Parameters.AddWithValue("@NIVEL_FIDELIDAD", nivelFidelidad);
            command.Parameters.AddWithValue("@PUNTOS_FIDELIDAD", puntosFidelidad);
            command.Parameters.AddWithValue("@TELEFONO", telefono ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@DIRECCION", direccion ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@USUARIO", usuario ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EMAIL", email ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CONTRASENIA", EncriptarContrasenia(contrasenia));

            try
            {
                // Ejecutamos el procedimiento almacenado
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar cliente: {ex.Message}");
                return false;
            }
        }

        private string GenerarIdCliente()
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = new SqlCommand("SELECT TOP 1 ID_CLIENTE FROM CLIENTES ORDER BY ID_CLIENTE DESC", connection);
            var ultimoId = command.ExecuteScalar() as string;

            if (string.IsNullOrEmpty(ultimoId))
            {
                return "C001"; 
            }

            var numeroStr = ultimoId.Substring(1); 
            if (int.TryParse(numeroStr, out int numero))
            {
                return $"C{(numero + 1):D3}"; 
            }

            throw new Exception("Formato inválido en el ID del cliente."); 
        }

        private string EncriptarContrasenia(string contrasenia)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contrasenia));
            return Convert.ToBase64String(bytes); // Convertimos el hash en un formato legible
        }

        public bool ActualizarCliente(string idCliente, string nombres, string apellidos, int? idDoc, string docIdent, DateTime? fechaNacimiento, byte? nivelFidelidad, int? puntosFidelidad, string telefono, string direccion, string usuario, string email, string contrasenia)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_CRUD_Clientes", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Añadir los parámetros para el procedimiento almacenado
                    command.Parameters.AddWithValue("@Accion", 3);  // Acción de Actualizar
                    command.Parameters.AddWithValue("@ID_CLIENTE", idCliente ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NOMBRES", nombres ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@APELLIDOS", apellidos ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ID_DOC", idDoc ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DOC_IDENT", docIdent ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FECHA_NACIMIENTO", fechaNacimiento ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NIVEL_FIDELIDAD", nivelFidelidad ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PUNTOS_FIDELIDAD", puntosFidelidad ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@TELEFONO", telefono ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DIRECCION", direccion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@USUARIO", usuario ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EMAIL", email ?? (object)DBNull.Value);

                    // Verificar si la contraseña fue proporcionada para encriptarla
                    if (!string.IsNullOrEmpty(contrasenia))
                    {
                        command.Parameters.AddWithValue("@CONTRASENIA", EncriptarContrasenia(contrasenia));
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@CONTRASENIA", DBNull.Value);
                    }

                    // Abrir conexión y ejecutar el comando
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    // Retornar si se actualizó al menos una fila
                    return rowsAffected > 0;
                }
            }
        }


        public bool EliminarCliente(string idCliente)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_CRUD_Clientes", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Accion", 4);
                    command.Parameters.AddWithValue("@ID_CLIENTE", idCliente);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool ActivarCliente(string idCliente)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_CRUD_Clientes", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Accion", 5);
                    command.Parameters.AddWithValue("@ID_CLIENTE", idCliente);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }




    }
}
