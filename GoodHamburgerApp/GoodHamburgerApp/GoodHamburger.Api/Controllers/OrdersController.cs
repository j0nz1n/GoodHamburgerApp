using GoodHamburger.Api.Data;
using GoodHamburger.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        public OrdersController(AppDbContext context) => _context = context;

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(List<Guid> menuItemIds)
        {
            var order = new Order();
            foreach (var menuItemId in menuItemIds) 
            { 
                var menuItem = await _context.MenuItems.FindAsync(menuItemId);
                if (menuItem == null) return BadRequest($"Menu item with id {menuItemId} not found.");

                if (order.OrderItems.Any(oi => (int)oi.Type == (int)menuItem.Type))
                {
                    return BadRequest($"Order already contains an item of type {menuItem.Type}.");

                }
                
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.OrderId,
                    MenuItemId = menuItem.MenuItemId,
                    Name = menuItem.Name,
                    Price = menuItem.Price,
                    Type = menuItem.Type
                };
                order.OrderItems.Add(orderItem);
            }

            order.TotalPrice = order.CalculateTotalPrice();
            order.Price = order.OrderItems.Sum(x=>x.Price);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null) return NotFound();
            return order;
        }
    }
}
    

