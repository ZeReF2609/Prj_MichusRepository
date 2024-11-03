using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class RegistroIngreso
{
    public int IdIngreso { get; set; }

    public DateTime FechaIngreso { get; set; }

    public int? IdMetodoPago { get; set; }

    public decimal Monto { get; set; }

    public virtual MetodoPago? IdMetodoPagoNavigation { get; set; }
}
