using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class Cliente
{
    public string IdCliente { get; set; } = null!;

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public int IdDoc { get; set; }

    public string DocIdent { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public DateTime? FechaUltimaCompra { get; set; }

    public byte NivelFidelidad { get; set; }

    public int PuntosFidelidad { get; set; }

    public virtual TipoDocumento IdDocNavigation { get; set; } = null!;

    public virtual ICollection<Opinione> Opiniones { get; set; } = new List<Opinione>();

    public virtual ICollection<PuntosFidelidad> PuntosFidelidads { get; set; } = new List<PuntosFidelidad>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
