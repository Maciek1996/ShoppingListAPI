using Microsoft.EntityFrameworkCore;
using ShoppingListAPI.DbContexts;
using ShoppingListAPI.Entities;
using ShoppingListAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services
{
    public class ShoppingListRepository : IShoppingListRepository
    {
        private readonly ShoppingListContext _context;
        private readonly IProductRepository _productRepository;
        private readonly ITagRepository _tagRepository;

        public ShoppingListRepository(ShoppingListContext context, IProductRepository productRepository, ITagRepository tagRepository)
        {
            _context = context;
            _productRepository = productRepository;
            _tagRepository = tagRepository;
        }

        public ShoppingList AddMultipleProductsToList(IEnumerable<ProductsList> products, Guid? tagId)
        {
            var list = GetCurrentList(tagId);
            if (list == null)
            {
                return null;
            }

            var productsIds = products.Select(p => p.ProductId).ToList();
            var currentProductsIds = list.ProductsList.Select(pl => pl.ProductId).ToList();
            var availableProductsIds = _context.Products.Select(p => p.Id).ToList();

            List<Guid> Ids  = productsIds.Except(currentProductsIds).ToList();
            Ids = Ids.Intersect(availableProductsIds).ToList();

            var productsToAdd = products.Where(p=> Ids.Contains(p.ProductId)).ToList();

            foreach (var product in productsToAdd)
            {
                _context.ProductsList.Add(new ProductsList() { ShoppingListId = list.Id, ProductId = product.ProductId, Pieces = product.Pieces, Weight = product.Weight, Type = product.Type });
            }

            _context.SaveChanges();
            list = GetCurrentList(tagId);

            return list;
        }

        public Status AddProductToList(Guid productId, Guid? tagId, int pieces, double weight)
        {
            var list = GetCurrentList(tagId);
            if (list == null)
            {
                return Status.NotFound;
            }

            var existingProduct = _productRepository.GetProduct(productId);
            if (existingProduct == null)
            {
                return Status.NotFound;
            }


            var productList =  _context.ProductsList.Where( pl => pl.ShoppingListId == list.Id && pl.ProductId == productId ).FirstOrDefault();

            if (productList != null)
            {
                return Status.Exists;
            }

            var newProductList = new ProductsList() { ProductId = productId, ShoppingListId = list.Id, Type = existingProduct.Type};
            
            if (existingProduct.Type == QuantityType.Piece)
            {
                if (pieces > 0)
                {
                    newProductList.Pieces = pieces;
                }
                else if (pieces == 0)
                {
                    newProductList.Pieces = 1;
                }
                else
                {
                    newProductList.Pieces = -1 * pieces;
                }
            }

            if (existingProduct.Type == QuantityType.Weight)
            {
                if (weight > 0)
                {
                    newProductList.Weight = weight;
                }
                else if (weight < 0)
                {
                    newProductList.Weight = -1.0 * weight;
                }
            }

            _context.ProductsList.Add(newProductList);
            _context.SaveChanges();
            return Status.Success;
        }

        public Status ChangeProductState(Guid productId, Guid? tagId, bool? isBought, int? pieces, double? weight, QuantityType? type)
        {
            var list = GetCurrentList(tagId);

            ProductsList productList = _context.ProductsList.Where(pl => pl.ShoppingListId == list.Id && pl.ProductId == productId).FirstOrDefault();
            if (productList == null)
            {
                return Status.NotFound;
            }

            if (isBought != null)
            {
                productList.IsBought = (bool)isBought;
            }
            if (type != null)
            {
                productList.Type = (QuantityType)type;
            }
            if (pieces != null && productList.Type == QuantityType.Piece)
            {
                productList.Pieces = (int)pieces;
            }
            if (weight != null && productList.Type == QuantityType.Weight)
            {
                productList.Weight = (double)weight;
            }
            _context.ProductsList.Attach(productList).State = EntityState.Modified;
            _context.SaveChanges();
            return Status.Success;
        }

        public Status CreateNewList(Guid? tagId, out IEnumerable<ProductsList> products)
        {
            var list = GetCurrentList(tagId);

            products = new List<ProductsList>();
            if (list == null)
            {
                return Status.NotFound;
            }
            if (list.ProductsList.Count == 0)
            {             
                return Status.NoItems;
            }

            products = list.ProductsList.Where(pl => pl.IsBought == false).ToList();
            ShoppingList newList = new ShoppingList();
            newList.ListTagId = tagId;
            _context.Lists.Add(newList);
            _context.SaveChanges();

            return Status.Success;
        }

        public Status DeleteProductFromList(Guid productId, Guid? tagId)
        {
            var list = GetCurrentList(tagId);
            if (list == null)
            {
                return Status.NotFound;
            }

            var productList = _context.ProductsList.Where(pl => pl.ProductId == productId && pl.ShoppingListId == list.Id).FirstOrDefault();
            if (productList != null)
            {
                _context.ProductsList.Attach(productList).State = EntityState.Deleted;
                _context.SaveChanges();
                return Status.Success;
            }
            return Status.NotFound;
        }

        public ShoppingList GetCurrentList(Guid? tagId)
        {
            Tag tag = null;
            if (tagId != null)
            {
                tag = _tagRepository.GetTag((Guid)tagId);
            }
            
            var list = _context.Lists.Include(l=>l.ProductsList).ThenInclude(pl => pl.Product).ThenInclude(p=>p.Unit).OrderByDescending(l => l.CreationDate).Where(l => l.ListTagId== tagId).FirstOrDefault();

            if (list == null)
            {
                if (tagId != null && tag == null)
                {
                    return null;
                }

                var newList = new ShoppingList();
                if (tag != null)
                {
                    newList.ListTag = tag;
                }
                _context.Lists.Add(newList);
                _context.SaveChanges();
                list = newList;
            }

            return list;   
        }

        public IEnumerable<ShoppingList> GetLists()
        {
            var lists = _context.Lists.Include(l => l.ListTag).Include(l => l.ProductsList).ThenInclude(pl => pl.Product).OrderByDescending(l => l.CreationDate).AsEnumerable();
            return lists;
        }

        public IEnumerable<UnitOfMeasurement> GetAllUnits()
        {
            var units = _context.Units.AsEnumerable();

            return units;
        }
    }
}
