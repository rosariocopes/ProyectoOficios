using AutoMapper;
using Oficios.Applications.Dtos.PaymentType;
using Oficios.Applications.Dtos.PaymentType.Oficios.Applications.Dtos.PaymentType;
using Oficios.Entities;

namespace Oficios.WebApi.Mapping
{
    
        public class PaymentTypeMappingProfile : Profile
        {
            public PaymentTypeMappingProfile()
            {
                CreateMap<PaymentType, PaymentTypeResponseDto>();

                CreateMap<PaymentTypeRequestDto, PaymentType>();
            }
        }
    }

