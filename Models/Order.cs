using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Cafee_Prototype.Models;

public class Order
{
    public int OrderId { get; set; }
    public bool IsOrderComplete { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    
    [Required]
    public IEnumerable<OrderItem> OrderItems{ get; set; } = new List<OrderItem>();

    public int? TableId { get; set; }
    public Table? Table { get; set; }
}