using System;
using System.Collections.Generic;

namespace Michus.Models;

public partial class Producto
{
    public string IdProducto { get; set; } = null!;

    public string ProdNom { get; set; } = null!;

    public string ProdNomweb { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string IdCategoria { get; set; } = null!;

    public DateOnly? ProdFchcmrl { get; set; }

    public decimal Precio { get; set; }

    public int Estado { get; set; }
    public string? Imagen { get; set; }


    public virtual Categorium IdCategoriaNavigation { get; set; } = null!;
}
