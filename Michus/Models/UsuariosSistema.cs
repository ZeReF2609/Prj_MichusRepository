using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class UsuariosSistema
{
    public string IdUsuario { get; set; } = null!;

    public string Usuario { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Contrasenia { get; set; } = null!;

    public string IdRol { get; set; } = null!;

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public byte Estado { get; set; }

    public virtual ICollection<CalificacionesOpinione> CalificacionesOpiniones { get; set; } = new List<CalificacionesOpinione>();

    public virtual Role IdRolNavigation { get; set; } = null!;
}
