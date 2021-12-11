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

        public ShoppingList AddMultipleProductsToList(IEnumerable<Product> products, Guid? tagId)
        {
            var list = GetCurrentList(tagId);
            if (list == null)
            {
                return null;
            }

            var productsIds = products.Select(p => p.Id).ToList();
            var currentProductsIds = list.ProductsList.Select(pl => pl.ProductId).ToList();
            var availableProductsIds = _context.Products.Select(p => p.Id).ToList();

            List<Guid> Ids  = productsIds.Except(currentProductsIds).ToList();
            Ids = Ids.Intersect(availableProductsIds).ToList();

            foreach (var productId in Ids)
            {
                _context.ProductsList.Add(new ProductsList() { ShoppingListId = list.Id, ProductId = productId });
            }

            _context.SaveChanges();
            list = GetCurrentList(tagId);

            return list;
        }

        public Status AddProductToList(Guid productId, Guid? tagId)
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

            var newProductList = new ProductsList() { ProductId = productId, ShoppingListId = list.Id};
            _context.ProductsList.Add(newProductList);
            _context.SaveChanges();
            return Status.Success;
        }

        public Status ChangeProductState(Guid productId, Guid? tagId, bool isBought)
        {
            var list = GetCurrentList(tagId);

            ProductsList productList = _context.ProductsList.Where(pl => pl.ShoppingListId == list.Id && pl.ProductId == productId).FirstOrDefault();
            if (productList == null)
            {
                return Status.NotFound;
            }

            productList.IsBought = isBought;
            _context.ProductsList.Attach(productList).State = EntityState.Modified;
            _context.SaveChanges();
            return Status.Success;
        }

        public Status CreateNewList(Guid? tagId, out IEnumerable<Product> products)
        {
            var list = GetCurrentList(tagId);

            products = new List<Product>();
            if (list == null)
            {
                return Status.NotFound;
            }
            if (list.ProductsList.Count == 0)
            {             
                return Status.NoItems;
            }

            products = list.ProductsList.Where(pl => pl.IsBought == false).Select(pl => pl.Product).ToList();
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
            
            var list = _context.Lists.Include(l=>l.ProductsList).ThenInclude(pl => pl.Product).OrderByDescending(l => l.CreationDate).Where(l => l.ListTagId== tagId).FirstOrDefault();

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
    }
}
