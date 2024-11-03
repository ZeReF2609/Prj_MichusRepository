using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class InventarioLog
{
    public int IdLog { get; set; }

    public int IdAlmacen { get; set; }

    public byte TipoMovimiento { get; set; }

    public int CantidadMovimiento { get; set; }

    public DateTime? FechaMovimiento { get; set; }

    public string? DescripcionMovimiento { get; set; }
}
