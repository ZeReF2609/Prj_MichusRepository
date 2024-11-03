using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class Empleado
{
    public string IdEmpleado { get; set; } = null!;

    public string Nombres { get; set; } = null!;

    public string? Apellidos { get; set; }

    public DateOnly FechaNacimiento { get; set; }

    public int IdDoc { get; set; }

    public string? DocIdent { get; set; }

    public decimal? Salario { get; set; }

    public DateTime FechaIngreso { get; set; }

    public byte Estado { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public virtual TipoDocumento IdDocNavigation { get; set; } = null!;
}
