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

        }

        private IEnumerable<Models.ProductListDto> GetList(Entities.ShoppingList Shoppinglist)
        {
            List<Models.ProductListDto> list = new List<Models.ProductListDto>();
            var tmpList = Shoppinglist.ProductsList;
            foreach (var item in tmpList)
            {
                var productList = new Models.ProductListDto();
                productList.Id = item.ProductId;
                productList.Name = item.Product.Name;
                productList.Description = item.Product.Description;
                productList.IsBought = item.IsBought;
                list.Add(productList);
            }
            return list;
        }
    }
}
