using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
            command.Parameters.AddWithValue("@CONTRASENIA", (contrasenia) ?? (object)DBNull.Value);

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
        public ClienteCompleto ObtenerClientePorId(string idCliente)
        {
            ClienteCompleto? clienteCompleto = null;  // Aquí guardamos el cliente encontrado

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("sp_CRUD_Clientes", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Parámetro de acción
                command.Parameters.AddWithValue("@Accion", 2); // Acción 2 para obtener cliente(s)

                // Parámetro opcional ID_CLIENTE
                if (!string.IsNullOrEmpty(idCliente))
                {
                    command.Parameters.AddWithValue("@ID_CLIENTE", idCliente); // Le pasamos el ID del cliente
                }
                else
                {
                    command.Parameters.AddWithValue("@ID_CLIENTE", DBNull.Value); // Si no se pasa el ID, lo dejamos como DBNull
                }

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    // Si encontramos al menos un cliente (suponiendo que la consulta retorna solo un cliente)
                    if (reader.Read())
                    {
                        Cliente cliente = new Cliente
                        {
                            IdCliente = reader["CODIGO"].ToString(),
                            Nombres = reader["NOMBRES"].ToString(),
                            Apellidos = reader["APELLIDOS"].ToString(),
                            IdDoc = Convert.ToInt32(reader["TIPO"]),
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
                            IdRol = reader["ID_ROL"].ToString(),
                            Email = reader["EMAIL"] != DBNull.Value ? reader["EMAIL"].ToString() : string.Empty,  // Asignamos el email
                            Usuario = reader["USUARIO"] != DBNull.Value ? reader["USUARIO"].ToString() : string.Empty  // Asignamos el nombre de usuario
                        };


                        // Crear una instancia de ClienteCompleto con toda la información y asignarlo
                        clienteCompleto = new ClienteCompleto
                        {
                            Cliente = cliente,
                            Contacto = contacto,
                            Sistema = usuarioSistema
                        };
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return clienteCompleto; // Si no se encontró el cliente, devolverá null
        }



        public bool ActualizarCliente(int accion, string idCliente, string nombres, string apellidos, int? idDoc, string docIdent, DateTime? fechaNacimiento, byte? nivelFidelidad, int? puntosFidelidad, string telefono, string direccion, string usuario, string email)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_CRUD_Clientes", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Añadir los parámetros para el procedimiento almacenado
                    // Para el procedimiento almacenado, añadir los parámetros en el mismo orden
                    command.Parameters.AddWithValue("@Accion", 3);  // @Accion - Acción de Actualizar
                    command.Parameters.AddWithValue("@ID_CLIENTE", idCliente ?? (object)DBNull.Value); // @ID_CLIENTE
                    command.Parameters.AddWithValue("@NOMBRES", nombres ?? (object)DBNull.Value);  // @NOMBRES
                    command.Parameters.AddWithValue("@APELLIDOS", apellidos ?? (object)DBNull.Value); // @APELLIDOS
                    command.Parameters.AddWithValue("@ID_DOC", idDoc ?? (object)DBNull.Value); // @ID_DOC
                    command.Parameters.AddWithValue("@DOC_IDENT", docIdent ?? (object)DBNull.Value); // @DOC_IDENT
                    command.Parameters.AddWithValue("@FECHA_NACIMIENTO", fechaNacimiento ?? (object)DBNull.Value); // @FECHA_NACIMIENTO
                    command.Parameters.AddWithValue("@NIVEL_FIDELIDAD", nivelFidelidad ?? (object)DBNull.Value); // @NIVEL_FIDELIDAD
                    command.Parameters.AddWithValue("@PUNTOS_FIDELIDAD", puntosFidelidad ?? (object)DBNull.Value); // @PUNTOS_FIDELIDAD
                    command.Parameters.AddWithValue("@TELEFONO", telefono ?? (object)DBNull.Value); // @TELEFONO
                    command.Parameters.AddWithValue("@DIRECCION", direccion ?? (object)DBNull.Value); // @DIRECCION
                    command.Parameters.AddWithValue("@USUARIO", usuario ?? (object)DBNull.Value); // @USUARIO
                    command.Parameters.AddWithValue("@EMAIL", email ?? (object)DBNull.Value); // @EMAIL

                    // Abrir conexión y ejecutar el comando
                    connection.Open();  
                    int rowsAffected = command.ExecuteNonQuery();

                    try
                    {
                        // Ejecutamos el procedimiento almacenado
                        command.ExecuteNonQuery();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al actualizar cliente: {ex.Message}");
                        return false;
                    }   
                }
            }
        }


        public bool EliminarCliente(string idCliente)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_CRUD_Clientes", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Accion", 4);  // Acción de eliminar
                        command.Parameters.AddWithValue("@ID_CLIENTE", idCliente);  // ID del cliente

                        connection.Open();
                        command.ExecuteNonQuery();  // Ejecutamos el procedimiento sin verificar filas afectadas

                        return true;  // Si no ocurre excepción, consideramos que la operación fue exitosa
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error: {ex.Message}");
                return false;  // Si ocurre una excepción, la operación falla
            }
        }

        public bool ActivarCliente(string idCliente)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_CRUD_Clientes", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Accion", 5);  // Acción de activar
                        command.Parameters.AddWithValue("@ID_CLIENTE", idCliente);  // ID del cliente

                        connection.Open();
                        command.ExecuteNonQuery();  // Ejecutamos el procedimiento sin verificar filas afectadas

                        return true;  // Si no ocurre excepción, consideramos que la operación fue exitosa
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error: {ex.Message}");
                return false;  // Si ocurre una excepción, la operación falla
            }
        }



    }
}
