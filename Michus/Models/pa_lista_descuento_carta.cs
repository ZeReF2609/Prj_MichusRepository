namespace Michus.Models
{
    public class pa_lista_descuento_carta
    {
        public string IdDescuento { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal PrecioDescuento { get; set; }
        public byte TipoDescuento { get; set; }
        public int Estado { get; set; }
        public string? TiSitu { get; set; }
       
    }
}
