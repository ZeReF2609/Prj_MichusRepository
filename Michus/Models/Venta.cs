using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class Venta
{
    public string IdVenta { get; set; } = null!;

    public string? IdUsuario { get; set; }

    public DateTime FechaVenta { get; set; }

    public decimal MontoTotal { get; set; }

    public int IdMetodoPago { get; set; }

    public int Estado { get; set; }

    public virtual MetodoPago IdMetodoPagoNavigation { get; set; } = null!;

    public virtual Cliente? IdUsuarioNavigation { get; set; }
    public List<VentaDetalle> Detalles { get; set; }
}
