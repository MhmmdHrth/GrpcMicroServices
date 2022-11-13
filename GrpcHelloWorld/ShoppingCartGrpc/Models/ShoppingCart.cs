
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCartGrpc.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public List<ShoppingCartItem> Items { get; set; } = new();

        public ShoppingCart()
        {
        }

        public ShoppingCart(string userName)
        {
        }

        public float TotalPrice
        {
            get
            {
                return Items.Sum(x => x.Price * x.Quantity);
            }
        }
    }
}
