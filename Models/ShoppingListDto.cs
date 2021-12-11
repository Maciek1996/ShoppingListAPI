using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListAPI.Models
{
    public class ShoppingListDto
    {
        public Guid Id { get; set; }
        public string CreationDate { get; set; }
        public string TagName { get; set; }
        public IEnumerable<ProductListDto> ProductLists { get; set; } = new List<ProductListDto>();

    }
}
