using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Michus.Models;

namespace Michus.DAO
{
    public class ReservasDAO
    {
        private readonly string _connectionString;

        public ReservasDAO(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        // Método para ejecutar procedimientos almacenados y retornar el resultado
        private bool EjecutarSP(int accion, string nombreUsuario, string idMesa, DateOnly? fechaReserva, TimeOnly? horaReserva, int? cantidadPersonas)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_Reservas", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Accion", accion);
                        command.Parameters.AddWithValue("@Nombre_Usuario", nombreUsuario ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ID_Mesa", idMesa ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Fecha_Reserva", fechaReserva?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Hora_Reserva", horaReserva?.ToString("HH:mm:ss") ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Cantidad_Personas", cantidadPersonas ?? (object)DBNull.Value);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();  // Ejecutar procedimiento y obtener filas afectadas

                        return rowsAffected > 0;  // Si hubo filas afectadas, la operación fue exitosa
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error: {ex.Message}");
                return false;  // En caso de error, la operación falla
            }
        }

        // Acción 1: Crear reserva
        public bool CrearReserva(string nombreUsuario, string idMesa, DateOnly fechaReserva, TimeOnly horaReserva, int cantidadPersonas)
        {
            return EjecutarSP(1, nombreUsuario, idMesa, fechaReserva, horaReserva, cantidadPersonas);
        }

        // Acción 2: Listar reservas (utilizando Dapper en el ejemplo anterior, aquí usaremos otro enfoque)
        public IEnumerable<dynamic> ListarReservas()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_Reservas", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Accion", 2);  // Acción para listar reservas

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            var reservas = new List<dynamic>();

                            // Leer los resultados
                            while (reader.Read())
                            {
                                reservas.Add(new
                                {
                                    ID_Reserva = reader["ID_Reserva"],
                                    Nombre_Usuario = reader["Nombre_Usuario"],
                                    ID_Mesa = reader["ID_Mesa"],
                                    Numero_Mesa = reader["Numero_Mesa"],
                                    Asientos = reader["Asientos"],
                                    Capacidad = reader["Asientos"],  // Add the 'Capacidad' property
                                    Fecha_Reserva = reader["Fecha_Reserva"],
                                    Hora_Reserva = reader["Hora_Reserva"],
                                    Estado_Mesa = reader["Estado_Mesa"],
                                    Disponibilidad_Mesa = reader["Disponibilidad_Mesa"],
                                    Mensaje = reader["Mensaje"]
                                });

                            }

                            return reservas;  // Retorna las reservas encontradas
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<dynamic>();  // Si ocurre un error, retorna una lista vacía
            }
        }


        // Acción 3: Liberar mesa
        public bool LiberarMesa(string nombreUsuario, string idMesa)
        {
            return EjecutarSP(3, nombreUsuario, idMesa, null, null, null);
        }

        // Acción 4: Verificar si la mesa existe y está disponible
        public bool VerificarDisponibilidadMesa(string idMesa)
        {
            return EjecutarSP(4, null, idMesa, null, null, null);
        }

        // Acción 5: Actualizar reserva
        public bool ActualizarReserva(string idReserva, string nombreUsuario, DateOnly? fechaReserva, TimeOnly? horaReserva, int? cantidadPersonas)
        {
            return EjecutarSP(5, nombreUsuario, null, fechaReserva, horaReserva, cantidadPersonas);
        }

        // Acción 6: Eliminar reserva
        public bool EliminarReserva(string idReserva)
        {
            return EjecutarSP(6, null, null, null, null, null);
        }

        // Método para listar las mesas
        public IEnumerable<dynamic> ListarMesas()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_LISTARMESAS", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            var mesas = new List<dynamic>();

                            while (reader.Read())
                            {
                                mesas.Add(new
                                {
                                    ID_Mesa = reader["ID_MESA"],
                                    NumeroMesa = reader["NUMERO_MESA"],
                                    Asientos = reader["ASIENTOS"],
                                    Estado = reader["ESTADO"],
                                    Disponibilidad = reader["DISPONIBILIDAD"]
                                });
                            }

                            return mesas; 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<dynamic>();  // En caso de error, retorna una lista vacía
            }
        }
    }
}
