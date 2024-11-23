namespace Michus.Models
{
    public class Descuento
    {
        public string? IdDescuento { get; set; }
        public int? IdPromocion { get; set; }
        public string? IdEvento { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal PrecioDescuento { get; set; }
        public byte TipoDescuento { get; set; } // 0 = Fijo, 1 = Porcentual
        public bool AplicarCategoria { get; set; }
        public string? IdCategoria { get; set; }
        public string? IdArticulos { get; set; }
        public string? TI_SITU { get; set; }
    }
}
