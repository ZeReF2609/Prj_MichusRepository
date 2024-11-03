using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class VentasNoRegistrada
{
    public string IdVenta { get; set; } = null!;

    public DateTime FechaVenta { get; set; }

    public decimal MontoTotal { get; set; }

    public int IdMetodoPago { get; set; }

    public int Estado { get; set; }

    public virtual MetodoPago IdMetodoPagoNavigation { get; set; } = null!;
}
