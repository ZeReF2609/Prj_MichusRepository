using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class DescuentoConfig
{
    public string IdDescuento { get; set; } = null!;

    public string? IdCategoria { get; set; }

    public string? Valor { get; set; }

    public virtual DescuentoCab IdDescuentoNavigation { get; set; } = null!;
}
