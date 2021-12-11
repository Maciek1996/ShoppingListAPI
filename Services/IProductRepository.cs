using ShoppingListAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services
{
    public interface IProductRepository
    {
        public void AddProduct(Product product);

        public Product GetProduct(Guid productId);

        public IEnumerable<Product> GetProducts();

        public IEnumerable<Product> GetProducts(string searchQuery);

        public void UpdateProduct(Product product);

        public bool DeleteProduct(Guid productId);

    }
}
