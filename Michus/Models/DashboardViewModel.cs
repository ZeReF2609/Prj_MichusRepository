using System.Collections.Generic;

namespace Michus.Models
{
    public class DashboardViewModel
    {
        public int TotalSales { get; set; }
        public int TotalProducts { get; set; }
        public int TotalClients { get; set; }
        public List<CategoryProductCount> ProductCountByCategory { get; set; }
        public List<CategoryProductValue> ProductValueByCategory { get; set; }
    }

    public class CategoryProductCount
    {
        public string Category { get; set; }
        public int ProductCount { get; set; }
    }

    public class CategoryProductValue
    {
        public string Category { get; set; }
        public decimal TotalValue { get; set; }
    }
}
//ACTUALIZADO A LA FECHA 26/11/2024