using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListAPI.Entities
{
    public class UnitOfMeasurement
    {
        public UnitOfMeasurement()
        {
            Id = Guid.NewGuid();
            IsActive = true;
        }

        [Key]
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string UnitName { get; set; }
        public string UnitCode { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
