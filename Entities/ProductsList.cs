using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListAPI.Entities
{
    public class ProductsList
    {
        public ProductsList()
        {
            IsBought = false;
        }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public bool IsBought { get; set; }
        public int Pieces { get; set; }
        public double Weight { get; set; }

        public Guid ShoppingListId { get; set; }
        public ShoppingList ShoppingList { get; set; }
    }
}
