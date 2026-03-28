namespace GoodHamburger.Api.Models
{
    public class OrderItem
    {
        public Guid Id { get; init; }
        public Guid OrderId { get; init; }
        public Guid MenuItemId { get; init; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public TypeItem Type { get; set; }
        public Order Order { get; set; }
        public MenuItem MenuItem { get; set; }
    }
}
