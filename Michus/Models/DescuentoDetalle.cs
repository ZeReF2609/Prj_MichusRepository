using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class DescuentoDetalle
{
    public string IdDescuento { get; set; } = null!;

    public string? IdArticulos { get; set; }

    public decimal PrecioFinal { get; set; }

    public int? Tipo { get; set; }

    public int? CantidadAplicable { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public byte? Estado { get; set; }
}
