namespace Michus.Models
{
    public class ClienteRegistroModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string IdCliente { get; set; }
        public int IdDoc { get; set; }
        public string DocIdent { get; set; }
        public DateTime FechaNacimiento { get; set; }
    }
}
