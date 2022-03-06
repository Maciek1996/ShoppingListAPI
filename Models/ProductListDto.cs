using ShoppingListAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListAPI.Models
{
    public class ProductListDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsBought { get; set; }
        public int Pieces { get; set; }
        public double Weight { get; set; }
        public string Unit { get; set; }
        public QuantityType Type { get; set; }
    }
}
