using Microsoft.AspNetCore.Mvc;
using Michus.DAO;
using Michus.Service;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Michus.Models;
using System.Diagnostics;
using Michus.Models_Store;

namespace Michus.Controllers
{
    public class ClientesController : Controller
    {
        private readonly ClientesDAO _clientesDao;
        private readonly MenuService _menuService;

        // Inyección de dependencias
        public ClientesController(ClientesDAO clientesDao, MenuService menuService)
        {
            _clientesDao = clientesDao;
            _menuService = menuService;
        }

        [HttpGet("Usuarios/lista-clientes")]
        [Authorize]
        public IActionResult ListaClientes()
        {
            try
            {
                // Obtener todos los clientes
                var clientes = _clientesDao.ObtenerTodosClientes();
                LoadMenuDataAsync();
                if (clientes == null || clientes.Count == 0)
                {
                    return NotFound("No se encontraron clientes.");
                }

                return View(clientes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los datos de los clientes: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }

        }

        public IActionResult ObtenerTiposDocumento()
        {
            try
            {
                // Llamar al DAO para obtener los tipos de documentos de forma síncrona
                var tiposDocumento = _clientesDao.ObtenerTiposDocumento();

                // Verificar si la lista tiene elementos
                if (tiposDocumento == null || !tiposDocumento.Any())
                {
                    return NotFound(new { mensaje = "No se encontraron tipos de documento." });
                }
                return Ok(tiposDocumento);
            }
            catch (Exception ex)
            {
                // En caso de error, retornar una respuesta con el código de error 500
                return StatusCode(500, new { mensaje = "Error al obtener tipos de documento", error = ex.Message });
            }
        }


        [Authorize]
        public IActionResult GuardarCliente([FromBody] ClienteCompleto clienteCompleto)
        {
            try
            {
                // Validar que los datos necesarios están presentes
                if (clienteCompleto == null || clienteCompleto.Sistema == null || clienteCompleto.Contacto == null)
                {
                    return BadRequest("Los datos del cliente, usuario y contacto son requeridos.");
                }

                // Convertir DateOnly a DateTime (si es necesario)
                var fechaNacimiento = clienteCompleto.Cliente.FechaNacimiento != default
                    ? clienteCompleto.Cliente.FechaNacimiento.ToDateTime(new TimeOnly(0, 0))
                    : DateTime.MinValue;

                // Incrementar el nivel de fidelidad
                var nivelFidelidad = clienteCompleto.Cliente.NivelFidelidad + 1;

                // Generar un nuevo cliente pasando la acción como 1
                var resultado = _clientesDao.InsertarCliente(
                    clienteCompleto.Sistema.Usuario,
                    clienteCompleto.Sistema.Email,
                    clienteCompleto.Sistema.Contrasenia,
                    clienteCompleto.Cliente.Nombres ?? string.Empty,
                    clienteCompleto.Cliente.Apellidos ?? string.Empty,
                    clienteCompleto.Cliente.IdDoc,
                    clienteCompleto.Cliente.DocIdent ?? string.Empty,
                    fechaNacimiento,
                    nivelFidelidad,
                    clienteCompleto.Cliente.PuntosFidelidad,
                    clienteCompleto.Contacto.Telefono ?? string.Empty,
                    clienteCompleto.Contacto.Direccion ?? string.Empty,
                    1 
                );

                // Devolver el resultado adecuado
                if (resultado)
                {
                    return Ok(new { success = true, mensaje = "Cliente guardado correctamente." });
                }
                else
                {
                    return StatusCode(500, new { success = false, mensaje = "Error al guardar el cliente." });
                }
            }
            catch (Exception ex)
            {
                // Manejo de error
                return StatusCode(500, new { success = false, mensaje = "Error al procesar la solicitud", error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult ActualizarCliente(ClienteCompleto clienteCompleto)
        {
            // Validar que el objeto clienteCompleto y sus propiedades no sean nulos
            if (clienteCompleto == null || clienteCompleto.Cliente == null || clienteCompleto.Contacto == null || clienteCompleto.Sistema == null)
            {
                return BadRequest("Los datos del cliente son inválidos.");
            }

            // Asegurarse de que el ID del cliente es válido
            if (string.IsNullOrEmpty(clienteCompleto.Cliente.IdCliente))
            {
                return BadRequest("El ID del cliente no puede estar vacío.");
            }

            // Convertir DateOnly a DateTime, manejar valores nulos correctamente
            DateTime? fechaNacimiento = clienteCompleto.Cliente.FechaNacimiento != default
                ? clienteCompleto.Cliente.FechaNacimiento.ToDateTime(new TimeOnly(0, 0))
                : (DateTime?)null;

            // Llamar al método del Dao para actualizar los datos del cliente
            bool actualizado = _clientesDao.ActualizarCliente(
                clienteCompleto.Cliente.IdCliente,         // idCliente
                clienteCompleto.Cliente.Nombres,          // nombres
                clienteCompleto.Cliente.Apellidos,        // apellidos
                null,                                     // idDoc (ajustar si es necesario)
                clienteCompleto.Cliente.DocIdent,         // docIdent
                fechaNacimiento,                          // fechaNacimiento
                clienteCompleto.Cliente.NivelFidelidad,   // nivelFidelidad
                clienteCompleto.Cliente.PuntosFidelidad,  // puntosFidelidad
                clienteCompleto.Contacto.Telefono,        // telefono
                clienteCompleto.Contacto.Direccion,       // direccion
                clienteCompleto.Sistema.IdUsuario,        // usuario
                clienteCompleto.Sistema.Email,            // email
                clienteCompleto.Sistema.Contrasenia       // contrasenia
            );


            // Retornar el resultado de la operación
            if (actualizado)
            {
                return Ok("Cliente actualizado correctamente.");
            }
            else
            {
                return StatusCode(500, "Hubo un problema al actualizar el cliente.");
            }
        }

        public IActionResult EliminarCliente(string idCliente)
        {
            if (string.IsNullOrEmpty(idCliente))
            {
                return BadRequest("El ID del cliente no puede estar vacío.");
            }

            bool eliminado = _clientesDao.EliminarCliente(idCliente);

            if (eliminado)
            {
                return Ok("Cliente eliminado correctamente.");
            }
            else
            {
                return StatusCode(500, "Hubo un problema al eliminar el cliente.");
            }
        }

        public IActionResult ActivarCliente(string idCliente)
        {
            if (string.IsNullOrEmpty(idCliente))
            {
                return BadRequest("El ID del cliente no puede estar vacío.");
            }

            bool activado = _clientesDao.ActivarCliente(idCliente);

            if (activado)
            {
                return Ok("Cliente activado correctamente.");
            }
            else
            {
                return StatusCode(500, "Hubo un problema al activar el cliente.");
            }
        }

        private async Task LoadMenuDataAsync()
        {
            string roleId = GetCurrentRoleId();
            var menuJson = await _menuService.GetMenusJsonByRoleAsync(roleId);
            ViewData["MenuItems"] = string.IsNullOrEmpty(menuJson) ? "[]" : menuJson;
        }

        private string GetCurrentRoleId()
        {
            var roleIdClaim = User.FindFirst(ClaimTypes.Role);
            return roleIdClaim?.Value ?? string.Empty;
        }
    }
}
