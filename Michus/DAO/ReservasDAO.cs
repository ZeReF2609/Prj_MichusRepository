using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
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

        private bool EjecutarSP(int accion, string? nombreUsuario, string idMesa, DateOnly? fechaReserva, TimeOnly? horaReserva, int? cantidadPersonas, string? idReserva = null)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@Accion", accion);

                    parameters.Add("@Nombre_Usuario", string.IsNullOrEmpty(nombreUsuario) ? null : nombreUsuario);
                    parameters.Add("@ID_Mesa", string.IsNullOrEmpty(idMesa) ? null : idMesa);
                    parameters.Add("@Fecha_Reserva", fechaReserva.HasValue ? (object)fechaReserva.Value.ToString("yyyy-MM-dd") : null);
                    parameters.Add("@Hora_Reserva", horaReserva.HasValue ? (object)horaReserva.Value.ToString("HH:mm:ss") : null);
                    parameters.Add("@Cantidad_Personas", cantidadPersonas.HasValue ? (object)cantidadPersonas.Value : null);
                    parameters.Add("@ID_Reserva", string.IsNullOrEmpty(idReserva) ? null : idReserva);


                    var rowsAffected = connection.Execute("SP_Reservas", parameters, commandType: CommandType.StoredProcedure);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la ejecución del SP: {ex.Message}");
                return false;
            }
        }


        public bool CrearReserva(string nombreUsuario, string idMesa, DateOnly fechaReserva, TimeOnly horaReserva, int cantidadPersonas, string? idReserva = null)
        {
            return EjecutarSP(1, nombreUsuario, idMesa, fechaReserva, horaReserva, cantidadPersonas,idReserva);
        }
        public IEnumerable<dynamic> ListarReservas(string? idReserva = null)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("@Accion", 2);
                    var reservas = connection.Query<dynamic>("SP_Reservas", parameters, commandType: CommandType.StoredProcedure);
                    return reservas;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<dynamic>(); 
            }
        }

        public bool LiberarMesa(string idReserva)
        {
            return EjecutarSP(3, null, null, null, null, null, idReserva);
        }

        // Método para listar mesas
        public IEnumerable<dynamic> ListarMesas()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var mesas = connection.Query<dynamic>("SP_LISTARMESAS", commandType: CommandType.StoredProcedure);
                    return mesas;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<dynamic>();
            }
        }
    }
}
