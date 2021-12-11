using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListAPI.Entities
{
    public class ShoppingList
    {
        public ShoppingList()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.Now;
        }


        [Key]
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public ICollection<ProductsList> ProductsList { get; set; } = new List<ProductsList>();
        public Tag ListTag { get; set; }
        public Guid? ListTagId { get; set; }
    }
}
