using System.ComponentModel.DataAnnotations;

namespace GoodHamburger.Api.Models
{
    public class Order
    {
        public Guid OrderId { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public decimal Price { get; set; } 
        public decimal TotalPrice { get; set; } 
        public List<OrderItem> OrderItems { get; set; } = new();

        public decimal CalculateTotalPrice()
        {
            
            decimal subtotal = OrderItems.Sum(oi => oi.Price);

            
            bool hasHamburger = OrderItems.Any(oi => oi.Type == TypeItem.Hamburger);
            bool hasFries = OrderItems.Any(oi => oi.Name == "Fries");
            bool hasSoftDrink = OrderItems.Any(oi => oi.Name == "Soft Drink");

            decimal discount = 0;
            if (hasHamburger && hasFries && hasSoftDrink) discount = 0.20m;
            else if (hasHamburger && hasSoftDrink) discount = 0.15m;
            else if (hasHamburger && hasFries) discount = 0.10m;

            return subtotal * (1 - discount);
        }
        public void AddItem(MenuItem menu)
        {
            
            if (OrderItems.Any(oi => oi.Name == menu.Name))
                throw new Exception($"O item {menu.Name} já está no pedido.");

            var item = new OrderItem
            {
                Id = Guid.NewGuid(),
                MenuItemId = menu.MenuItemId,
                Name = menu.Name,
                Price = menu.Price,
                
            };

            OrderItems.Add(item);
            this.TotalPrice = CalculateTotalPrice();
        }
    }

}

