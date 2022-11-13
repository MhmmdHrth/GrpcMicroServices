using DiscountGrpc.Models;
using System.Collections.Generic;

namespace DiscountGrpc.Data
{
    public class DiscountContext
    {
        public static readonly IEnumerable<Discount> Discounts = new List<Discount>
        {
                new Discount { DiscountId = 1, Amount = 100, Code = "CODE_100"},
                new Discount { DiscountId = 2, Amount = 200, Code = "CODE_200"},
                new Discount { DiscountId = 3, Amount = 300, Code = "CODE_300"}
        };
    }
}
