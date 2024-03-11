using AutoMapper;
using Payment.Application.ViewModels;
using Payment.Domain.Models;

namespace Payment.Application.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreatePaymentCardViewModel, Domain.Models.Payment>()
                .ConstructUsing(src => new Domain.Models.Payment()
                {
                    UserId = src.UserId,
                    Card = new Card()
                    {
                        ClientName = src.ClientName,
                        Number = src.Number,
                        DateValidate = src.DateValidate,
                        SecurityCode = src.SecurityCode
                    }
                });

            CreateMap<CreatePaymentPixViewModel, Domain.Models.Payment>()
                .ConstructUsing(src => new Domain.Models.Payment()
                {
                    UserId = src.UserId,
                    Pix = new Pix()
                    {
                        Key = src.Key
                    }
                });
        }
    }
}
