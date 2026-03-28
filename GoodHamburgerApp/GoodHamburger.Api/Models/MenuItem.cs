using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoodHamburger.Api.Models
{
    public class MenuItem
    {
        
        public Guid MenuItemId { get; init; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public TypeItem Type { get; set; }
        


    }
}
