using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ShoppingListAPI.Entities;
using ShoppingListAPI.Models;
using ShoppingListAPI.Services;
using ShoppingListAPI.Utilities;

namespace ShoppingListAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    [EnableCors]
    public class ShoppingListController : ControllerBase
    {
        private readonly IShoppingListRepository _shoppingListRepository;
        private readonly IMapper _mapper;

        public ShoppingListController(IShoppingListRepository shoppingListRepository, IMapper mapper)
        {
            _shoppingListRepository = shoppingListRepository;
            _mapper = mapper;
        }

        [HttpGet("list", Name ="CurrentList")]
        public ActionResult<IEnumerable<ProductListDto>> GetCurrentList([FromQuery] Guid? tagId)
        {
            var curentListEntity = _shoppingListRepository.GetCurrentList(tagId);
            if (curentListEntity == null)
            {
                return NotFound();
            }
            var currentList = _mapper.Map<ShoppingListDto>(curentListEntity).ProductLists.ToList();
            return currentList;
        }

        [HttpGet("lists")]
        public ActionResult<IEnumerable<ShoppingListDto>> GetLists()
        {
            var lists = _shoppingListRepository.GetLists();
            return _mapper.Map<IEnumerable<ShoppingListDto>>(lists).ToList();
        }

        [HttpPost("list")]
        public ActionResult<IEnumerable<ProductDto>> AddNewList([FromQuery] Guid? tagId)
        {
            IEnumerable<ProductsList> products;
            var result = _shoppingListRepository.CreateNewList(tagId, out products);
            if (result == Status.NotFound)
            {
                return NotFound("Nie znaleziono listy.");
            }
            if (result == Status.NoItems)
            {
                return NotFound("Lista jest pusta, nie można utworzyć nowej.");
            }

            if (result == Status.Success)
            {
                var list = _mapper.Map<IEnumerable<ProductListDto>>(products).ToList();
                return Created("", list);
            }

            return BadRequest();
        }

        [HttpPut("list/{productId}")]
        public IActionResult AddProductToList(Guid productId, [FromQuery] Guid? tagId, [FromQuery] int pieces, [FromQuery] double weight )
        {

            var result = _shoppingListRepository.AddProductToList(productId, tagId, pieces, weight);
            if (result == Status.NotFound)
            {
                return NotFound();
            }
            else if (result == Status.Success)
            {
                return CreatedAtRoute("CurrentList", null);
            }
            else if (result == Status.Exists)
            {
                return BadRequest("Produkt jest już na wskazanej liście zakupów.");
            }
            return BadRequest();

        }

        [HttpDelete("list/{productId}")]
        public IActionResult DeleteProductFromList(Guid productId, [FromQuery] Guid? tagId)
        {
            var result = _shoppingListRepository.DeleteProductFromList(productId, tagId);
            if (result == Status.NotFound)
            {
                return NotFound();
            }
            else if (result == Status.Success)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut("list")]
        public IActionResult AddProductsToList([FromBody] IEnumerable<ProductListDto> products, [FromQuery] Guid? tagId)
        {
            var list = _shoppingListRepository.AddMultipleProductsToList(_mapper.Map<IEnumerable<ProductsList>>(products), tagId);
            if (list == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ShoppingListDto>(list));
        }

        [HttpPut("list/{ProductId}/state")]
        public IActionResult ChangeProductState(Guid ProductId, [FromQuery] Guid? tagId, [FromQuery] bool? IsBought, [FromQuery] int? pieces, [FromQuery] double? weight)
        {
            var result = _shoppingListRepository.ChangeProductState(ProductId, tagId, IsBought, pieces, weight);
            if (result == Status.NotFound)
            {
                return NotFound();
            }
            else if (result == Status.Success)
            {
                return Ok();
            }

            return BadRequest();

        }

    }
}
