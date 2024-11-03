using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class DescuentoCab
{
    public string IdDescuento { get; set; } = null!;

    public int? IdPromocion { get; set; }

    public string? IdEvento { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public decimal PrecioDescuento { get; set; }

    public byte TipoDescuento { get; set; }

    public int Estado { get; set; }

    public string? TiSitu { get; set; }

    public int TipoFil { get; set; }

    public virtual Evento? IdEventoNavigation { get; set; }

    public virtual Promocion? IdPromocionNavigation { get; set; }
}
