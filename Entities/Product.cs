using ShoppingListAPI.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListAPI.Entities
{
    public class Product
    {
        public Product()
        {
            Id = Guid.NewGuid();
            IsActive = true;
            Type = QuantityType.None;
        }

        [Key]
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public QuantityType Type { get; set; }
        public Guid? UnitId { get; set; }
        public UnitOfMeasurement Unit { get; set; }
        public ICollection<ProductsList> ProductsList { get; set; } = new List<ProductsList>();
    }
}
