using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Cafee_Prototype.Models;

public class OrderItem
{
    public int OrderItemId { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal ProductPrice { get; set; }
    public string? ProductImage { get; set; }
    public int Quantity { get; set; }

    [ForeignKey("OrderId")]
    public int OrderId { get; set; }
    public Order Order { get; set; }
}
