using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListAPI.Utilities
{
    public enum Status
    {
        NotFound = 0,
        Success = 1,
        Exists = 2,
        NoItems = 3
    }
}
