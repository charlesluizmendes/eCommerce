using AutoMapper;
using Catalog.Application.ViewModels;
using Catalog.Domain.Models;

namespace Catalog.Application.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductViewModel>();
            CreateMap<ProductViewModel, Product>();
        }
    }
}
