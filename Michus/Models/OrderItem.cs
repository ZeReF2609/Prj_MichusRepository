namespace Michus.Models;

public class OrderItem
{
    public int OrderItemId { get; set; }
    public string IdProducto { get; set; } = null!;
    public int OrderId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public virtual Producto Producto { get; set; } = null!;
    public virtual Order Order { get; set; } = null!;
}
