using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ShoppingListAPI.Entities;
using ShoppingListAPI.Models;
using ShoppingListAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    [EnableCors]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public ProductController(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        [HttpGet]
        public ActionResult<IEnumerable<ProductDto>> GetProducts([FromQuery (Name = "search")] string searchQuery)
        {
            var products = _repository.GetProducts(searchQuery);
            return _mapper.Map<IEnumerable<ProductDto>>(products).ToList();
        }

        [HttpGet("{productId}", Name = "GetProduct")]
        public ActionResult<ProductDto> GetProduct(Guid productId)
        {
            var product = _repository.GetProduct(productId);
            
            if (product == null)
            {
                return NotFound();
            }

            return _mapper.Map<ProductDto>(product);
        }


        [HttpPost]
        public ActionResult<ProductDto> CreateProduct(ProductCreationDto product)
        {
            var productEntity = _mapper.Map<Product>(product);
            _repository.AddProduct(productEntity);

            var productToReturn = _mapper.Map<ProductDto>(productEntity);

            return CreatedAtRoute("GetProduct", new { productId = productToReturn.Id }, productToReturn);            
        }

        [HttpPut("{productId}")]
        public ActionResult UpdateProduct(Guid productId, ProductEditionDto product, [FromQuery] bool? changeForAll)
        {

            var existingProduct = _repository.GetProduct(productId);
            if (existingProduct == null)
            {
                return NotFound();
            }

            _mapper.Map(product, existingProduct);

            var changeTypeForAll = changeForAll != null ? (bool)changeForAll : false;
            _repository.UpdateProduct(existingProduct, changeTypeForAll);

            return NoContent();
        }

        [HttpDelete("{productId}")]
        public IActionResult DeleteProduct(Guid productId)
        {
            var result = _repository.DeleteProduct(productId);
            if (!result)
            {
                return NotFound();
            }
            return Ok();
        }


    }
}
