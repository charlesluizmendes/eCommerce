using AutoMapper;
using Basket.Application.ViewModels;
using Basket.Domain.Models;

namespace Basket.Application.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AddItemViewModel, Item>()
                .ConstructUsing(src => new Item()
                {
                    Quantity = src.Quantity,
                    ProductId = src.ProductId,
                    Basket = new Domain.Models.Basket()
                    {
                        UserId = src.UserId
                    }
                });

            CreateMap<Domain.Models.Basket, BasketViewModel>();
            CreateMap<Item, ItemViewModel>();
        }
    }
}
