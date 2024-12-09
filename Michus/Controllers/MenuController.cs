using Michus.Service;
using Michus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Michus.Controllers
{
    [Authorize(Policy = "AdminOnly")] // Solo accesible para usuarios con la política 'AdminOnly'
    public class MenuController : Controller
    {
        private readonly MenuService _menuService;
        private readonly IConfiguration _configuration;

        public MenuController(MenuService menuService, IConfiguration configuration)
        {
            _configuration = configuration;
            _menuService = menuService;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardViewModel = new DashboardViewModel();

            await using (var connection = new SqlConnection(_configuration.GetConnectionString("cn1")))
            {
                await connection.OpenAsync();

                // Obtener total de ventas
                await using (var command = new SqlCommand("SP_GetTotalSales", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    dashboardViewModel.TotalSales = Convert.ToInt32(await command.ExecuteScalarAsync());
                }

                // Obtener total de productos
                await using (var command = new SqlCommand("SP_GetTotalProducts", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    dashboardViewModel.TotalProducts = Convert.ToInt32(await command.ExecuteScalarAsync());
                }

                // Obtener total de clientes
                await using (var command = new SqlCommand("SP_GetTotalClients", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    dashboardViewModel.TotalClients = Convert.ToInt32(await command.ExecuteScalarAsync());
                }

                // Obtener cantidad de productos por categoría
                dashboardViewModel.ProductCountByCategory = new List<CategoryProductCount>();
                await using (var command = new SqlCommand("SP_GetProductCountByCategory", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        dashboardViewModel.ProductCountByCategory.Add(new CategoryProductCount
                        {
                            Category = reader["CATEGORIA"].ToString(),
                            ProductCount = Convert.ToInt32(reader["ProductCount"])
                        });
                    }
                }

                // Obtener valor total de productos por categoría
                dashboardViewModel.ProductValueByCategory = new List<CategoryProductValue>();
                await using (var command = new SqlCommand("SP_GetTotalProductValueByCategory", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        dashboardViewModel.ProductValueByCategory.Add(new CategoryProductValue
                        {
                            Category = reader["CATEGORIA"].ToString(),
                            TotalValue = Convert.ToDecimal(reader["TotalValue"])
                        });
                    }
                }
            }

            SetNoCacheHeaders();

            // Carga el menú dinámico según el rol del usuario
            await LoadMenuDataAsync();
            return View(dashboardViewModel);
        }

        private string GetCurrentRoleId()
        {
            // Obtiene el rol actual del usuario autenticado
            var roleIdClaim = User.FindFirst(ClaimTypes.Role);
            return roleIdClaim?.Value ?? string.Empty;
        }

        private async Task LoadMenuDataAsync()
        {
            string roleId = GetCurrentRoleId();

            if (string.IsNullOrEmpty(roleId))
            {
                ViewData["MenuItems"] = "[]";
                return;
            }

            var menuJson = await _menuService.GetMenusJsonByRoleAsync(roleId);

            if (string.IsNullOrEmpty(menuJson) || !IsValidJson(menuJson))
            {
                menuJson = "[]";
            }

            ViewData["MenuItems"] = menuJson;
        }

        private bool IsValidJson(string json)
        {
            try
            {
                JsonDocument.Parse(json);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        private void SetNoCacheHeaders()
        {
            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, post-check=0, pre-check=0";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
        }
    }
}
