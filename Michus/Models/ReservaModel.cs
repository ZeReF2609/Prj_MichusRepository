namespace Michus.Models
{
    public class ReservaModel
    {
        public string CorreoElectronico { get; set; } = "";
        public string FechaReserva { get; set; } = "";
        public string HoraReserva { get; set; } = "";
        public string MesaID { get; set; } = "";
        public int CantidadPersonas { get; set; }
    }
}
