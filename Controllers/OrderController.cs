using Cafee_Prototype.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cafee_Prototype.Controllers;


public class OrderController:Controller
{
    private readonly CafeeDbContext _cafeeDbContext;
    public OrderController(CafeeDbContext cafeeDbContext)
    {
        _cafeeDbContext = cafeeDbContext;
    }

    [Authorize]
    public async Task<IActionResult> ListOrders()
    {
        var orders = await _cafeeDbContext.Orders.Include(o => o.OrderItems).Include(t => t.Table).ToListAsync();
        return View(orders);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CompleteOrder(int id)
    {
        var order = await _cafeeDbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
        order.IsOrderComplete = true;
        await _cafeeDbContext.SaveChangesAsync();
        return RedirectToAction("ListOrders");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _cafeeDbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
        {
            return BadRequest("Order is null");
        }
        _cafeeDbContext.Orders.Remove(order);
        await _cafeeDbContext.SaveChangesAsync();

        return RedirectToAction("ListOrders");
    }

    public async Task<IActionResult> MakeOrder()
    {
        var list =  await _cafeeDbContext.ProductCategories.Include(c => c.Products).ToListAsync();
        return View(list);
    }

    [HttpPost]
    public async Task<IActionResult> MakeOrder(Dictionary<int, int> ProductAmounts, int id)
    {
        var table = await _cafeeDbContext.Tables.FirstOrDefaultAsync(c => c.TableId == id);

        var selectedProducts = ProductAmounts
        .Where(pa => pa.Value > 0)
        .Select(pa => new { ProductId = pa.Key, Amount = pa.Value })
        .ToList();

        var products = await _cafeeDbContext.Products
        .Where(p => selectedProducts.Select(sp => sp.ProductId).Contains(p.ProductId))
        .ToListAsync();

        Order order = new Order
        {
            Table = table,
            TableId = id,
            IsOrderComplete = false,
            OrderItems = products.Select(p => new OrderItem
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductImage = p.ProductImage,
                ProductPrice = p.ProductPrice,
                Quantity = selectedProducts.First(sp => sp.ProductId == p.ProductId).Amount
            }).ToList(),
        };

        await _cafeeDbContext.Orders.AddAsync(order);
        await _cafeeDbContext.SaveChangesAsync();

        return RedirectToAction("OrderSuccess", "Order");
    }

    public IActionResult OrderSuccess()
    {
        return View();
    }
}
