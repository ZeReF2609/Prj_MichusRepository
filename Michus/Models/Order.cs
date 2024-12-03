namespace Michus.Models;

public class Order
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public decimal TotalAmount { get; set; }
    public int Estado { get; set; } // Example: 0 for pending, 1 for completed, etc.

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
