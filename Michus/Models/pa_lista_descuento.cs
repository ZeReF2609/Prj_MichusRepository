namespace Michus.Models
{
    public class pa_lista_descuento
    {
        public string IdDescuento { get; set; } = null!;
        public int? IdPromocion { get; set; }
        public string? IdEvento { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal PrecioDescuento { get; set; }
        public byte TipoDescuento { get; set; }
        public string? IdCategoria { get; set; }
        public List<string>? IdArticulos { get; set; } = null;
        public int Estado { get; set; }
        public string? TiSitu { get; set; }
       
    }
}
