using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class Cuenta
{
    public string IdEmpleado { get; set; } = null!;

    public byte TipoCuenta { get; set; }

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;
}
