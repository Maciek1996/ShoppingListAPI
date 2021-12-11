using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListAPI.Entities
{
    public class Tag
    {
        public Tag()
        {
            Id = Guid.NewGuid();
            IsActive = true;
        }

        public Guid Id { get; set; }
        public string TagName { get; set; }
        public bool IsActive { get; set; }
        public ICollection<ShoppingList> ShoppingLists { get; set; } = new List<ShoppingList>();
    }
}
