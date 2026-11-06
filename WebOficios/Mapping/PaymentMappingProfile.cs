using AutoMapper;
using Oficios.Applications.Dtos.Payment;
using Oficios.Entities;

namespace Oficios.WebApi.Mapping
{
    public class PaymentMappingProfile : Profile
    {
        public PaymentMappingProfile()
        {
            CreateMap<Payment, PaymentResponseDto>()
                .ForMember(
                    dest => dest.PaymentTypeName,
                    opt => opt.MapFrom(src => src.PaymentType.Name)
                );
            CreateMap<Payment, PaymentResponseDto>()
    .ForMember(dest => dest.JobTitle,
        opt => opt.MapFrom(src => src.Job.Title))
    .ForMember(dest => dest.PaymentTypeName,
        opt => opt.MapFrom(src => src.PaymentType.Name));
        }
    }
}
