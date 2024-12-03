using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class Reserva
{
    public string IdReserva { get; set; } = null!;

    public string IdUsuario { get; set; } = null!;

    public string IdMesa { get; set; } = null!;

    public DateOnly FechaReserva { get; set; }

    public TimeOnly HoraReserva { get; set; }

    public int TiSitu { get; set; }

    public DateTime? FechaActualizacion { get; set; }

}
