using AutoMapper;
using Identity.Application.ViewModels;
using Identity.Domain.Models;

namespace Identity.Application.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AccessTokenViewModel, AccessToken>();
            CreateMap<AccessToken, AccessTokenViewModel>()
                .ForMember(dto => dto.TokenExpires, opt => opt.MapFrom(entity => entity.Expires));

            CreateMap<CreateAccessTokenViewModel, User>()
                .ForMember(entity => entity.PasswordHash, opt => opt.MapFrom(dto => dto.Password));
            CreateMap<User, CreateAccessTokenViewModel>()
                .ForMember(dto => dto.Password, opt => opt.MapFrom(entity => entity.PasswordHash));

            CreateMap<UserViewModel, User>();
            CreateMap<User, UserViewModel>()
                .ForMember(dto => dto.Name, opt => opt.MapFrom(entity => entity.UserName));

            CreateMap<CreateUserViewModel, User>()
                .ForMember(entity => entity.UserName, opt => opt.MapFrom(dto => dto.Name))
                .ForMember(entity => entity.PasswordHash, opt => opt.MapFrom(dto => dto.Password));
            CreateMap<User, CreateUserViewModel>()
                .ForMember(dto => dto.Name, opt => opt.MapFrom(entity => entity.UserName));

            CreateMap<UpdateUserViewModel, User>()
                .ForMember(entity => entity.UserName, opt => opt.MapFrom(dto => dto.Name))
                .ForMember(entity => entity.PasswordHash, opt => opt.MapFrom(dto => dto.Password));
            CreateMap<User, UpdateUserViewModel>()
                .ForMember(dto => dto.Name, opt => opt.MapFrom(entity => entity.UserName));
        }
    }
}
