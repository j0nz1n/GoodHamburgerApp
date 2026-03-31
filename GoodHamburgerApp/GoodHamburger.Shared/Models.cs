using System;
using System.Collections.Generic;
using System.Linq;

namespace GoodHamburger.Shared.Models;

public enum TypeItem { Hamburger, Extra }

public class MenuItem
{
    public Guid MenuItemId { get; init; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public TypeItem Type { get; set; }
}

public class Order
{
    public Guid OrderId { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    
    public decimal Price { get; set; }

    
    public decimal TotalPrice { get; set; }

    public List<OrderItem> OrderItems { get; set; } = new();

    public void UpdateTotals()
    {
        decimal subtotal = OrderItems.Sum(oi => oi.Price);
        this.Price = subtotal;

        
        bool hasHamburger = OrderItems.Any(oi => oi.Type == TypeItem.Hamburger);
        bool hasFries = OrderItems.Any(oi => oi.Name.Contains("Fries", StringComparison.OrdinalIgnoreCase));
        bool hasSoftDrink = OrderItems.Any(oi => oi.Name.Contains("Soft Drink", StringComparison.OrdinalIgnoreCase));

        decimal discount = 0;
        if (hasHamburger && hasFries && hasSoftDrink) discount = 0.20m;
        else if (hasHamburger && hasSoftDrink) discount = 0.15m;
        else if (hasHamburger && hasFries) discount = 0.10m;

        this.TotalPrice = subtotal * (1 - discount);
    }
}

public class OrderItem
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid OrderId { get; init; }
    public Guid MenuItemId { get; init; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public TypeItem Type { get; set; }

   
}