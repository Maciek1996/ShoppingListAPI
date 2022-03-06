using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListAPI.Profiles
{
    public class ShoppingListProfile : Profile
    {
        public ShoppingListProfile()
        {
            CreateMap<Entities.Product, Models.ProductDto>().ReverseMap();
            CreateMap<Models.ProductEditionDto, Entities.Product>();
            CreateMap<Models.ProductCreationDto, Entities.Product>();

            CreateMap<Entities.ShoppingList, Models.ShoppingListDto>()
                .ForMember(
                dest => dest.CreationDate,
                opt => opt.MapFrom(src => src.CreationDate.Date.ToShortDateString()))
                .ForMember(
                dest => dest.TagName,
                opt => opt.MapFrom(src => src.ListTag != null ? src.ListTag.TagName : null))
                .ForMember(
                dest => dest.ProductLists,
                opt => opt.MapFrom(src => GetList(src)));

            CreateMap<Models.TagCreationDto, Entities.Tag>();
            CreateMap<Models.TagEditionDto, Entities.Tag>();
            CreateMap<Entities.Tag, Models.TagDto>().ReverseMap();
            CreateMap<Entities.ProductsList, Models.ProductListDto>().ForMember(dest => dest.Unit, opt => opt.MapFrom(src =>GetUnit(src))).ReverseMap();
        }

        private string GetUnit(Entities.ProductsList productsList)
        {
            return productsList?.Product?.Unit?.UnitCode;
        }

        private IEnumerable<Models.ProductListDto> GetList(Entities.ShoppingList Shoppinglist)
        {
            List<Models.ProductListDto> list = new List<Models.ProductListDto>();
            var tmpList = Shoppinglist.ProductsList;
            foreach (var item in tmpList)
            {
                var productList = new Models.ProductListDto();
                productList.ProductId = item.ProductId;
                productList.Name = item.Product.Name;
                productList.Description = item.Product.Description;
                productList.IsBought = item.IsBought;
                productList.Type = item.Product.Type;
                productList.Pieces = item.Pieces;
                productList.Weight = item.Weight;
                productList.Unit = GetUnit(item);
                list.Add(productList);
            }
            return list;
        }
    }
}
