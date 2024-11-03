using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class MetodoPago
{
    public int IdMetodoPago { get; set; }

    public string Metodo { get; set; } = null!;

    public virtual ICollection<RegistroIngreso> RegistroIngresos { get; set; } = new List<RegistroIngreso>();

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();

    public virtual ICollection<VentasNoRegistrada> VentasNoRegistrada { get; set; } = new List<VentasNoRegistrada>();
}
