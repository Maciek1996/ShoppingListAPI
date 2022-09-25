using ShoppingListAPI.Entities;
using ShoppingListAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services
{
    public interface IShoppingListRepository
    {
        public ShoppingList GetCurrentList(Guid? tagId);

        public IEnumerable<ShoppingList> GetLists();

        public Status AddProductToList(Guid productId, Guid? tagId, int pieces, double weight);

        public Status DeleteProductFromList(Guid productId, Guid? tagId);

        public Status ChangeProductState(Guid productId, Guid? tagId, bool? isBought, int? pieces, double? weight, QuantityType? type);

        public Status CreateNewList(Guid? tagId, out IEnumerable<ProductsList> products);

        public ShoppingList AddMultipleProductsToList(IEnumerable<ProductsList> products, Guid? tagId);

        public IEnumerable<UnitOfMeasurement> GetAllUnits();

    }
}
