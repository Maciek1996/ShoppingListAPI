using ShoppingListAPI.DbContexts;
using ShoppingListAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services
{
    public class ProductRepository : IProductRepository
    {

        private readonly ShoppingListContext _context;

        public ProductRepository(ShoppingListContext context)
        {
            _context = context;
        }

        public void AddProduct(Product product)
        {
            if (product.UnitId == null && product.Type == Utilities.QuantityType.Weight)
            {
                UnitOfMeasurement unit = _context.Units.Where(u => u.UnitCode == "kg").FirstOrDefault();
                if (unit == null)
                {
                    _context.Units.Add(new UnitOfMeasurement { UnitName = "Kilogram", UnitCode = "kg" });
                    _context.SaveChanges();
                    unit = _context.Units.Where(u => u.UnitCode == "kg").FirstOrDefault();
                }
                product.Unit = unit;
            }
            _context.Add(product);
            _context.SaveChanges();
        }

        public bool DeleteProduct(Guid productId)
        {
            var product = GetProduct(productId);
            if (product != null)
            {
                var existsOnLists = _context.ProductsList.Where(pl => pl.ProductId == productId).FirstOrDefault();
                if (existsOnLists != null)
                {
                    product.IsActive = false;
                }
                else
                {
                    _context.Attach(product).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                }
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }


        public Product GetProduct(Guid productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            return product;
        }

        public IEnumerable<Product> GetProducts()
        {
            var products = _context.Products.Where(p => p.IsActive).AsEnumerable();
            return products;
        }

        public IEnumerable<Product> GetProducts(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return GetProducts();
            }

            searchQuery = searchQuery.Trim();

            var products = _context.Products as IQueryable<Product>;
            products = products.Where(p => p.Name.Contains(searchQuery) && p.IsActive);
            var list = products.ToList();
            return list;
        }

        public void UpdateProduct(Product product)
        {
            if (product.UnitId == null && product.Type == Utilities.QuantityType.Weight)
            {
                UnitOfMeasurement unit = _context.Units.Where(u => u.UnitCode == "kg").FirstOrDefault();
                if (unit == null)
                {
                    _context.Units.Add(new UnitOfMeasurement { UnitName = "Kilogram", UnitCode = "kg" });
                    _context.SaveChanges();
                    unit = _context.Units.Where(u => u.UnitCode == "kg").FirstOrDefault();
                }
                product.Unit = unit;
            }
            _context.SaveChanges();
        }
    }
}
