using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class Mesa
{
    public string IdMesa { get; set; } = null!;

    public int NumeroMesa { get; set; }

    public int Asientos { get; set; }

    public int Estado { get; set; }

    public int Disponibilidad { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
